using Cassandra.Application.Contracts.Biro;
using Cassandra.Domain.Biro;
using Cassandra.Domain.Biro.Events;
using Cassandra.Domain.Common;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Biro;

public class BiroRepository(AppDbContext context) : IBiroRepository
{
    private const string AggregateType = "Biro";

    public async Task<Domain.Biro.Biro?> GetByIdAsync(BiroId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Biro.Biro.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Biro.Biro biro, CancellationToken ct = default)
    {
        var newEvents = biro.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = biro.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = biro.DealerId,
                AggregateType = AggregateType,
                AggregateId   = biro.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(biro, newEvents, ct);
        await context.SaveChangesAsync(ct);
        biro.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Biro.Biro biro,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.BiroReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == biro.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<BiroCreated>().First();
            projection = new BiroReadModel
            {
                Id        = biro.Id.Value,
                DealerId  = biro.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.BiroReadModels.Add(projection);
        }

        if (newEvents.OfType<BiroUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<BiroActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<BiroDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name     = biro.Name;
        projection.Phone    = biro.Phone;
        projection.Fax      = biro.Fax;
        projection.Pic      = biro.Pic;
        projection.Address  = biro.Address;
        projection.PphRate  = biro.PphRate;
        projection.IsActive = biro.IsActive;
    }
}
