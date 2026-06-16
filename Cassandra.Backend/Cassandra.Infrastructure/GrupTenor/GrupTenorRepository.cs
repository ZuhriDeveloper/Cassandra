using Cassandra.Application.Contracts.GrupTenor;
using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTenor;
using Cassandra.Domain.GrupTenor.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.GrupTenor;

public class GrupTenorRepository(AppDbContext context) : IGrupTenorRepository
{
    private const string AggregateType = "GrupTenor";

    public async Task<Domain.GrupTenor.GrupTenor?> GetByIdAsync(GrupTenorId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.GrupTenor.GrupTenor.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.GrupTenor.GrupTenor gt, CancellationToken ct = default)
    {
        var newEvents = gt.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = gt.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = gt.DealerId,
                AggregateType = AggregateType,
                AggregateId   = gt.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(gt, newEvents, ct);
        await context.SaveChangesAsync(ct);
        gt.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.GrupTenor.GrupTenor gt,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.GrupTenorReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == gt.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<GrupTenorCreated>().First();
            projection = new GrupTenorReadModel
            {
                Id        = gt.Id.Value,
                DealerId  = gt.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.GrupTenorReadModels.Add(projection);
        }

        if (newEvents.OfType<GrupTenorUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<GrupTenorActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<GrupTenorDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name     = gt.Name;
        projection.IsActive = gt.IsActive;
    }
}
