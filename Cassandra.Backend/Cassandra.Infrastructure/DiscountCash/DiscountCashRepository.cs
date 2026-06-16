using Cassandra.Application.Contracts.DiscountCash;
using Cassandra.Domain.Common;
using Cassandra.Domain.DiscountCash;
using Cassandra.Domain.DiscountCash.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.DiscountCash;

public class DiscountCashRepository(AppDbContext context) : IDiscountCashRepository
{
    private const string AggregateType = "DiscountCash";

    public async Task<Domain.DiscountCash.DiscountCash?> GetByIdAsync(DiscountCashId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.DiscountCash.DiscountCash.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.DiscountCash.DiscountCash dc, CancellationToken ct = default)
    {
        var newEvents = dc.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = dc.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = dc.DealerId,
                AggregateType = AggregateType,
                AggregateId   = dc.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(dc, newEvents, ct);
        await context.SaveChangesAsync(ct);
        dc.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.DiscountCash.DiscountCash dc,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.DiscountCashReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == dc.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<DiscountCashCreated>().First();
            projection = new DiscountCashReadModel
            {
                Id          = dc.Id.Value,
                DealerId    = dc.DealerId,
                TipeMotorId = created.TipeMotorId,
                CreatedBy   = created.CreatedBy,
                CreatedAt   = created.OccurredAt,
            };
            context.DiscountCashReadModels.Add(projection);
        }

        if (newEvents.OfType<DiscountCashUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<DiscountCashActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<DiscountCashDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.DirectDiscount  = dc.DirectDiscount;
        projection.ChannelDiscount = dc.ChannelDiscount;
        projection.IsActive        = dc.IsActive;
    }
}
