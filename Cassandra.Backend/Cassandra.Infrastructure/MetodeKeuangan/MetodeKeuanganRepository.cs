using Cassandra.Application.Contracts.MetodeKeuangan;
using Cassandra.Domain.Common;
using Cassandra.Domain.MetodeKeuangan;
using Cassandra.Domain.MetodeKeuangan.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.MetodeKeuangan;

public class MetodeKeuanganRepository(AppDbContext context) : IMetodeKeuanganRepository
{
    private const string AggregateType = "MetodeKeuangan";

    public async Task<Domain.MetodeKeuangan.MetodeKeuangan?> GetByIdAsync(MetodeKeuanganId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.MetodeKeuangan.MetodeKeuangan.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.MetodeKeuangan.MetodeKeuangan mk, CancellationToken ct = default)
    {
        var newEvents = mk.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = mk.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = mk.DealerId,
                AggregateType = AggregateType,
                AggregateId   = mk.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(mk, newEvents, ct);
        await context.SaveChangesAsync(ct);
        mk.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.MetodeKeuangan.MetodeKeuangan mk,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.MetodeKeuanganReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == mk.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<MetodeKeuanganCreated>().First();
            projection = new MetodeKeuanganReadModel
            {
                Id        = mk.Id.Value,
                DealerId  = mk.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.MetodeKeuanganReadModels.Add(projection);
        }

        if (newEvents.OfType<MetodeKeuanganUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<MetodeKeuanganActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<MetodeKeuanganDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name     = mk.Name;
        projection.IsActive = mk.IsActive;
    }
}
