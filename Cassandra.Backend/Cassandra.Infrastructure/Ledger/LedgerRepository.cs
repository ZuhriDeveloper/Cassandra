using Cassandra.Application.Contracts.Ledger;
using Cassandra.Domain.Common;
using Cassandra.Domain.Ledger;
using Cassandra.Domain.Ledger.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Ledger;

public class LedgerRepository(AppDbContext context) : ILedgerRepository
{
    private const string AggregateType = "Ledger";

    public async Task<Domain.Ledger.Ledger?> GetByIdAsync(LedgerId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Ledger.Ledger.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Ledger.Ledger ledger, CancellationToken ct = default)
    {
        var newEvents = ledger.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = ledger.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = ledger.DealerId,
                AggregateType = AggregateType,
                AggregateId   = ledger.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(ledger, newEvents, ct);
        await context.SaveChangesAsync(ct);
        ledger.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Ledger.Ledger ledger,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.LedgerReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == ledger.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<LedgerCreated>().First();
            projection = new LedgerReadModel
            {
                Id        = ledger.Id.Value,
                DealerId  = ledger.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.LedgerReadModels.Add(projection);
        }

        if (newEvents.OfType<LedgerUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<LedgerActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<LedgerDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name     = ledger.Name;
        projection.IsActive = ledger.IsActive;
    }
}
