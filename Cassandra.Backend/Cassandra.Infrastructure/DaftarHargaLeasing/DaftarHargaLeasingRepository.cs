using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Domain.Common;
using Cassandra.Domain.DaftarHargaLeasing;
using Cassandra.Domain.DaftarHargaLeasing.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.DaftarHargaLeasing;

public class DaftarHargaLeasingRepository(AppDbContext context) : IDaftarHargaLeasingRepository
{
    private const string AggregateType = "DaftarHargaLeasing";

    public async Task<Domain.DaftarHargaLeasing.DaftarHargaLeasing?> GetByIdAsync(DaftarHargaLeasingId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.DaftarHargaLeasing.DaftarHargaLeasing.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.DaftarHargaLeasing.DaftarHargaLeasing dhl, CancellationToken ct = default)
    {
        var newEvents = dhl.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = dhl.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = dhl.DealerId,
                AggregateType = AggregateType,
                AggregateId   = dhl.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(dhl, newEvents, ct);
        await context.SaveChangesAsync(ct);
        dhl.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.DaftarHargaLeasing.DaftarHargaLeasing dhl,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.DaftarHargaLeasingReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == dhl.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<DaftarHargaLeasingCreated>().First();
            projection = new DaftarHargaLeasingReadModel
            {
                Id        = dhl.Id.Value,
                DealerId  = dhl.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.DaftarHargaLeasingReadModels.Add(projection);
        }

        if (newEvents.OfType<DaftarHargaLeasingUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<DaftarHargaLeasingActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<DaftarHargaLeasingDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        if (newEvents.OfType<DaftarHargaLeasingItemsSet>().LastOrDefault() is { } itemsSet)
        {
            // Replace all items for this DHL
            var existingItems = await context.DaftarHargaLeasingItemReadModels
                .Where(x => x.DaftarHargaLeasingId == dhl.Id.Value)
                .ToListAsync(ct);
            context.DaftarHargaLeasingItemReadModels.RemoveRange(existingItems);

            foreach (var item in itemsSet.Items)
            {
                context.DaftarHargaLeasingItemReadModels.Add(new DaftarHargaLeasingItemReadModel
                {
                    DaftarHargaLeasingId = dhl.Id.Value,
                    GrupTipeMotorId      = item.GrupTipeMotorId,
                    Subsidi              = item.Subsidi,
                    Incentive            = item.Incentive,
                    LainLain             = item.LainLain,
                });
            }

            projection.UpdatedBy = itemsSet.UpdatedBy;
            projection.UpdatedAt = itemsSet.OccurredAt;
        }

        projection.Name           = dhl.Name;
        projection.GlobalLeasingId = dhl.GlobalLeasingId;
        projection.GrupTenorId    = dhl.GrupTenorId;
        projection.IsActive       = dhl.IsActive;
    }
}
