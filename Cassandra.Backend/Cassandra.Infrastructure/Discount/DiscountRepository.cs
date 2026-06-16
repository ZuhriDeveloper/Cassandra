using Cassandra.Application.Contracts.Discount;
using Cassandra.Domain.Common;
using Cassandra.Domain.Discount;
using Cassandra.Domain.Discount.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Discount;

public class DiscountRepository(AppDbContext context) : IDiscountRepository
{
    private const string AggregateType = "Discount";

    public async Task<Domain.Discount.Discount?> GetByIdAsync(DiscountId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Discount.Discount.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Discount.Discount discount, CancellationToken ct = default)
    {
        var newEvents = discount.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = discount.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = discount.DealerId,
                AggregateType = AggregateType,
                AggregateId   = discount.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(discount, newEvents, ct);
        await context.SaveChangesAsync(ct);
        discount.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Discount.Discount discount,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.DiscountReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == discount.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<DiscountCreated>().First();
            projection = new DiscountReadModel
            {
                Id        = discount.Id.Value,
                DealerId  = discount.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.DiscountReadModels.Add(projection);
        }

        if (newEvents.OfType<DiscountUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<DiscountActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<DiscountDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        if (newEvents.OfType<DiscountItemsSet>().LastOrDefault() is { } itemsSet)
        {
            var existingItems = await context.DiscountItemReadModels
                .Where(x => x.DiscountId == discount.Id.Value)
                .ToListAsync(ct);
            context.DiscountItemReadModels.RemoveRange(existingItems);

            foreach (var item in itemsSet.Items)
            {
                context.DiscountItemReadModels.Add(new DiscountItemReadModel
                {
                    DiscountId      = discount.Id.Value,
                    GrupTipeMotorId = item.GrupTipeMotorId,
                    Amount          = item.Amount,
                });
            }

            projection.UpdatedBy = itemsSet.UpdatedBy;
            projection.UpdatedAt = itemsSet.OccurredAt;
        }

        projection.DaftarHargaLeasingId = discount.DaftarHargaLeasingId;
        projection.Level    = discount.Level;
        projection.IsActive = discount.IsActive;
    }
}
