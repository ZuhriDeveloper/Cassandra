using Cassandra.Application.Contracts.CashOutTransaction;
using Cassandra.Domain.CashOutTransaction;
using Cassandra.Domain.CashOutTransaction.Events;
using Cassandra.Domain.Common;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.CashOutTransaction;

public class CashOutTransactionRepository(AppDbContext context) : ICashOutTransactionRepository
{
    private const string AggregateType = "CashOutTransaction";

    public async Task<Domain.CashOutTransaction.CashOutTransaction?> GetByIdAsync(CashOutTransactionId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.CashOutTransaction.CashOutTransaction.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.CashOutTransaction.CashOutTransaction cashOutTransaction, CancellationToken ct = default)
    {
        var newEvents = cashOutTransaction.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = cashOutTransaction.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = cashOutTransaction.DealerId,
                AggregateType = AggregateType,
                AggregateId   = cashOutTransaction.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(cashOutTransaction, newEvents, ct);
        await context.SaveChangesAsync(ct);
        cashOutTransaction.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.CashOutTransaction.CashOutTransaction cashOutTransaction,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.CashOutTransactions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == cashOutTransaction.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<CashOutTransactionCreated>().First();
            projection = new CashOutTransactionReadModel
            {
                Id        = cashOutTransaction.Id.Value,
                DealerId  = cashOutTransaction.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.CashOutTransactions.Add(projection);
        }

        projection.TransactionType = cashOutTransaction.TransactionType;
        projection.SoId            = cashOutTransaction.SoId;
        projection.SoReturId       = cashOutTransaction.SoReturId;
        projection.Amount          = cashOutTransaction.Amount;
        projection.DfAmount        = cashOutTransaction.DfAmount;
        projection.TotalHariDf     = cashOutTransaction.TotalHariDf;
        projection.PaymentDate     = cashOutTransaction.PaymentDate;
        projection.PaymentMethod   = cashOutTransaction.PaymentMethod;
        projection.FInvoiceId      = cashOutTransaction.FInvoiceId;
        projection.IsClosed        = cashOutTransaction.IsClosed;
    }
}
