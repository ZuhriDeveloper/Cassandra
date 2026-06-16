using Cassandra.Application.Contracts.AlokasiDiskon;
using Cassandra.Domain.AlokasiDiskon;
using Cassandra.Domain.AlokasiDiskon.Events;
using Cassandra.Domain.Common;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.AlokasiDiskon;

public class AlokasiDiskonRepository(AppDbContext context) : IAlokasiDiskonRepository
{
    private const string AggregateType = "AlokasiDiskon";

    public async Task<Domain.AlokasiDiskon.AlokasiDiskon?> GetByIdAsync(AlokasiDiskonId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.AlokasiDiskon.AlokasiDiskon.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.AlokasiDiskon.AlokasiDiskon ad, CancellationToken ct = default)
    {
        var newEvents = ad.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = ad.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = ad.DealerId,
                AggregateType = AggregateType,
                AggregateId   = ad.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(ad, newEvents, ct);
        await context.SaveChangesAsync(ct);
        ad.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.AlokasiDiskon.AlokasiDiskon ad,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.AlokasiDiskonReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == ad.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<AlokasiDiskonCreated>().First();
            projection = new AlokasiDiskonReadModel
            {
                Id         = ad.Id.Value,
                DealerId   = ad.DealerId,
                KaryawanId = created.KaryawanId,
                CreatedBy  = created.CreatedBy,
                CreatedAt  = created.OccurredAt,
            };
            context.AlokasiDiskonReadModels.Add(projection);
        }

        if (newEvents.OfType<AlokasiDiskonUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<AlokasiDiskonActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<AlokasiDiskonDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.DiscountLevel = ad.DiscountLevel;
        projection.IsActive      = ad.IsActive;
    }
}
