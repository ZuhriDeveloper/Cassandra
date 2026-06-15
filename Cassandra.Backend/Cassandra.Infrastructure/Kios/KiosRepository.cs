using Cassandra.Application.Contracts.Kios;
using Cassandra.Domain.Common;
using Cassandra.Domain.Kios;
using Cassandra.Domain.Kios.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Kios;

public class KiosRepository(AppDbContext context) : IKiosRepository
{
    private const string AggregateType = "Kios";

    public async Task<Domain.Kios.Kios?> GetByIdAsync(KiosId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Kios.Kios.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Kios.Kios kios, CancellationToken ct = default)
    {
        var newEvents = kios.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = kios.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = kios.DealerId,
                AggregateType = AggregateType,
                AggregateId   = kios.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(kios, newEvents, ct);
        await context.SaveChangesAsync(ct);
        kios.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Kios.Kios kios,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.KiosReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == kios.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<KiosCreated>().First();
            projection = new KiosReadModel
            {
                Id        = kios.Id.Value,
                DealerId  = kios.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.KiosReadModels.Add(projection);
        }

        if (newEvents.OfType<KiosUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        projection.Name          = kios.Name;
        projection.Phone         = kios.Phone;
        projection.Fax           = kios.Fax;
        projection.Address       = kios.Address;
        projection.PicKaryawanId = kios.PicKaryawanId;
        projection.Limit         = kios.Limit;
        projection.IsActive      = kios.IsActive;
    }
}
