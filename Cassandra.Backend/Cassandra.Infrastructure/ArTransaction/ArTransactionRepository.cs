using Cassandra.Application.Contracts.ArTransaction;
using Cassandra.Domain.ArTransaction;
using Cassandra.Domain.ArTransaction.Events;
using Cassandra.Domain.Common;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.ArTransaction;

public class ArTransactionRepository(AppDbContext context) : IArTransactionRepository
{
    private const string AggregateType = "ArTransaction";

    public async Task<Domain.ArTransaction.ArTransaction?> GetByIdAsync(ArTransactionId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.ArTransaction.ArTransaction.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.ArTransaction.ArTransaction arTransaction, CancellationToken ct = default)
    {
        var newEvents = arTransaction.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = arTransaction.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = arTransaction.DealerId,
                AggregateType = AggregateType,
                AggregateId   = arTransaction.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(arTransaction, newEvents, ct);
        await context.SaveChangesAsync(ct);
        arTransaction.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.ArTransaction.ArTransaction arTransaction,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.ArTransactions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == arTransaction.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<ArTransactionCreated>().First();
            projection = new ArTransactionReadModel
            {
                Id        = arTransaction.Id.Value,
                DealerId  = arTransaction.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.ArTransactions.Add(projection);
        }

        projection.TransactionType  = arTransaction.TransactionType;
        projection.ReferenceId      = arTransaction.ReferenceId;
        projection.ReferenceNumber  = arTransaction.ReferenceNumber;
        projection.TotalAmount      = arTransaction.TotalAmount;
        projection.RemainingAmount  = arTransaction.RemainingAmount;
        projection.IsClosed         = arTransaction.IsClosed;

        // Sync owned payments
        projection.Payments.Clear();
        foreach (var p in arTransaction.Payments)
        {
            projection.Payments.Add(new ArPaymentEntryReadModel
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
