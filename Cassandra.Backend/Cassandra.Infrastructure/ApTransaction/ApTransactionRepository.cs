using Cassandra.Application.Contracts.ApTransaction;
using Cassandra.Domain.ApTransaction;
using Cassandra.Domain.ApTransaction.Events;
using Cassandra.Domain.Common;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.ApTransaction;

public class ApTransactionRepository(AppDbContext context) : IApTransactionRepository
{
    private const string AggregateType = "ApTransaction";

    public async Task<Domain.ApTransaction.ApTransaction?> GetByIdAsync(ApTransactionId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.ApTransaction.ApTransaction.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.ApTransaction.ApTransaction apTransaction, CancellationToken ct = default)
    {
        var newEvents = apTransaction.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = apTransaction.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = apTransaction.DealerId,
                AggregateType = AggregateType,
                AggregateId   = apTransaction.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(apTransaction, newEvents, ct);
        await context.SaveChangesAsync(ct);
        apTransaction.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.ApTransaction.ApTransaction apTransaction,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.ApTransactions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == apTransaction.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<ApTransactionCreated>().First();
            projection = new ApTransactionReadModel
            {
                Id        = apTransaction.Id.Value,
                DealerId  = apTransaction.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.ApTransactions.Add(projection);
        }

        projection.TransactionType  = apTransaction.TransactionType;
        projection.StnkId           = apTransaction.StnkId;
        projection.NoRangka         = apTransaction.NoRangka;
        projection.TotalAmount      = apTransaction.TotalAmount;
        projection.RemainingAmount  = apTransaction.RemainingAmount;
        projection.IsClosed         = apTransaction.IsClosed;

        // Sync owned payments
        projection.Payments.Clear();
        foreach (var p in apTransaction.Payments)
        {
            projection.Payments.Add(new ApPaymentEntryReadModel
            {
                PaymentNo     = p.PaymentNo,
                Amount        = p.Amount,
                PaymentDate   = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                FInvoiceId    = p.FInvoiceId,
                CreatedBy     = p.CreatedBy,
            });
        }
    }
}
