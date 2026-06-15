using Cassandra.Application.Contracts.Mediator;
using Cassandra.Domain.Common;
using Cassandra.Domain.Mediator;
using Cassandra.Domain.Mediator.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Mediator;

public class MediatorRepository(AppDbContext context) : IMediatorRepository
{
    private const string AggregateType = "Mediator";

    public async Task<Domain.Mediator.Mediator?> GetByIdAsync(MediatorId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Mediator.Mediator.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Mediator.Mediator mediator, CancellationToken ct = default)
    {
        var newEvents = mediator.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = mediator.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = mediator.DealerId,
                AggregateType = AggregateType,
                AggregateId   = mediator.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(mediator, newEvents, ct);
        await context.SaveChangesAsync(ct);
        mediator.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Mediator.Mediator mediator,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.MediatorReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == mediator.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<MediatorCreated>().First();
            projection = new MediatorReadModel
            {
                Id        = mediator.Id.Value,
                DealerId  = mediator.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.MediatorReadModels.Add(projection);
        }

        if (newEvents.OfType<MediatorUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        projection.Name       = mediator.Name;
        projection.KaryawanId = mediator.KaryawanId;
        projection.Address    = mediator.Address;
        projection.Limit      = mediator.Limit;
        projection.IsActive   = mediator.IsActive;
    }
}
