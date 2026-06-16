using Cassandra.Application.Contracts.BiayaBiroJasa;
using Cassandra.Domain.BiayaBiroJasa;
using Cassandra.Domain.BiayaBiroJasa.Events;
using Cassandra.Domain.Common;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.BiayaBiroJasa;

public class BiayaBiroJasaRepository(AppDbContext context) : IBiayaBiroJasaRepository
{
    private const string AggregateType = "BiayaBiroJasa";

    public async Task<Domain.BiayaBiroJasa.BiayaBiroJasa?> GetByIdAsync(BiayaBiroJasaId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.BiayaBiroJasa.BiayaBiroJasa.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.BiayaBiroJasa.BiayaBiroJasa bbj, CancellationToken ct = default)
    {
        var newEvents = bbj.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = bbj.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = bbj.DealerId,
                AggregateType = AggregateType,
                AggregateId   = bbj.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(bbj, newEvents, ct);
        await context.SaveChangesAsync(ct);
        bbj.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.BiayaBiroJasa.BiayaBiroJasa bbj,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.BiayaBiroJasaReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == bbj.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<BiayaBiroJasaCreated>().First();
            projection = new BiayaBiroJasaReadModel
            {
                Id        = bbj.Id.Value,
                DealerId  = bbj.DealerId,
                SamsatId  = bbj.SamsatId,
                BiroId    = bbj.BiroId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.BiayaBiroJasaReadModels.Add(projection);
        }

        if (newEvents.OfType<BiayaBiroJasaActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<BiayaBiroJasaDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        if (newEvents.OfType<BiayaBiroJasaItemsSet>().LastOrDefault() is { } itemsSet)
        {
            var existingItems = await context.BiayaBiroJasaItemReadModels
                .Where(x => x.BiayaBiroJasaId == bbj.Id.Value)
                .ToListAsync(ct);
            context.BiayaBiroJasaItemReadModels.RemoveRange(existingItems);

            foreach (var item in itemsSet.Items)
            {
                context.BiayaBiroJasaItemReadModels.Add(new BiayaBiroJasaItemReadModel
                {
                    BiayaBiroJasaId = bbj.Id.Value,
                    TipeMotorId     = item.TipeMotorId,
                    BiayaStnk       = item.BiayaStnk,
                    Notice          = item.Notice,
                });
            }

            projection.UpdatedBy = itemsSet.UpdatedBy;
            projection.UpdatedAt = itemsSet.OccurredAt;
        }

        projection.IsActive = bbj.IsActive;
    }
}
