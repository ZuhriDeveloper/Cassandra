using Cassandra.Application.Contracts.Df;
using Cassandra.Domain.Common;
using Cassandra.Domain.Df;
using Cassandra.Domain.Df.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Df;

public class DfRepository(AppDbContext context) : IDfRepository
{
    private const string AggregateType = "Df";

    public async Task<Domain.Df.Df?> GetForDealerAsync(Guid dealerId, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.DealerId == dealerId && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Df.Df.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Df.Df df, CancellationToken ct = default)
    {
        var newEvents = df.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = df.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = df.DealerId,
                AggregateType = AggregateType,
                AggregateId   = df.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(df, newEvents, ct);
        await context.SaveChangesAsync(ct);
        df.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Df.Df df,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.DfReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == df.Id.Value, ct);

        if (projection is null)
        {
            projection = new DfReadModel
            {
                Id       = df.Id.Value,
                DealerId = df.DealerId,
            };
            context.DfReadModels.Add(projection);
        }

        if (newEvents.OfType<DfSet>().LastOrDefault() is { } dfSet)
        {
            projection.Discount  = dfSet.Discount;
            projection.Interest  = dfSet.Interest;
            projection.StartDate = dfSet.StartDate;
            projection.UpdatedBy = dfSet.UpdatedBy;
            projection.UpdatedAt = dfSet.OccurredAt;
        }
    }
}
