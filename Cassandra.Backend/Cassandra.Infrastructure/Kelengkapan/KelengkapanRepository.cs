using Cassandra.Application.Contracts.Kelengkapan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Kelengkapan;
using Cassandra.Domain.Kelengkapan.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Kelengkapan;

public class KelengkapanRepository(AppDbContext context) : IKelengkapanRepository
{
    private const string AggregateType = "Kelengkapan";

    public async Task<Domain.Kelengkapan.Kelengkapan?> GetByIdAsync(KelengkapanId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Kelengkapan.Kelengkapan.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Kelengkapan.Kelengkapan kelengkapan, CancellationToken ct = default)
    {
        var newEvents = kelengkapan.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = kelengkapan.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = kelengkapan.DealerId,
                AggregateType = AggregateType,
                AggregateId   = kelengkapan.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(kelengkapan, newEvents, ct);
        await context.SaveChangesAsync(ct);
        kelengkapan.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Kelengkapan.Kelengkapan kelengkapan,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.KelengkapanReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == kelengkapan.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<KelengkapanCreated>().First();
            projection = new KelengkapanReadModel
            {
                Id        = kelengkapan.Id.Value,
                DealerId  = kelengkapan.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.KelengkapanReadModels.Add(projection);
        }

        if (newEvents.OfType<KelengkapanUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<KelengkapanActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<KelengkapanDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name     = kelengkapan.Name;
        projection.IsActive = kelengkapan.IsActive;
    }
}
