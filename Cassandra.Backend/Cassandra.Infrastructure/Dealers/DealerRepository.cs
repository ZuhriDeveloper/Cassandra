using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers;
using Cassandra.Domain.Dealers.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Dealers;

/// <summary>
/// Event-sourced repository for the <see cref="Dealer"/> aggregate. A dealer SELF-OWNS its
/// events: every stored event and the read-model row carry the dealer's own id, so the
/// non-null DealerId column holds even though the SuperAdmin creating the dealer has no
/// dealer scope of their own.
/// </summary>
public class DealerRepository(AppDbContext context) : IDealerRepository
{
    private const string AggregateType = "Dealer";

    public async Task<Dealer?> GetByIdAsync(DealerId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Dealer.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Dealer dealer, CancellationToken ct = default)
    {
        var newEvents = dealer.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = dealer.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = dealer.Id.Value,   // self-owned
                AggregateType = AggregateType,
                AggregateId   = dealer.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(dealer, newEvents, ct);
        await context.SaveChangesAsync(ct);
        dealer.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Dealer dealer,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.DealerReadModels.FindAsync([dealer.Id.Value], ct);

        if (projection is null)
        {
            var created = newEvents.OfType<DealerRegistered>().First();
            projection = new DealerReadModel
            {
                Id        = dealer.Id.Value,
                CreatedBy = created.RegisteredBy,
                CreatedAt = created.OccurredAt,
            };
            context.DealerReadModels.Add(projection);
        }

        projection.Name     = dealer.Name;
        projection.Code     = dealer.Code;
        projection.IsActive = dealer.IsActive;
    }
}
