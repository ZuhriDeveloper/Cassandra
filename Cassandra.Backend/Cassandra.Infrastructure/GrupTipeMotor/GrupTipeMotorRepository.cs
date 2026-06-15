using Cassandra.Application.Contracts.GrupTipeMotor;
using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTipeMotor;
using Cassandra.Domain.GrupTipeMotor.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.GrupTipeMotor;

public class GrupTipeMotorRepository(AppDbContext context) : IGrupTipeMotorRepository
{
    private const string AggregateType = "GrupTipeMotor";

    public async Task<Domain.GrupTipeMotor.GrupTipeMotor?> GetByIdAsync(GrupTipeMotorId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.GrupTipeMotor.GrupTipeMotor.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.GrupTipeMotor.GrupTipeMotor grup, CancellationToken ct = default)
    {
        var newEvents = grup.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = grup.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = grup.DealerId,
                AggregateType = AggregateType,
                AggregateId   = grup.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(grup, newEvents, ct);
        await context.SaveChangesAsync(ct);
        grup.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.GrupTipeMotor.GrupTipeMotor grup,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.GrupTipeMotorReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == grup.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<GrupTipeMotorCreated>().First();
            projection = new GrupTipeMotorReadModel
            {
                Id        = grup.Id.Value,
                DealerId  = grup.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.GrupTipeMotorReadModels.Add(projection);
        }

        if (newEvents.OfType<GrupTipeMotorActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<GrupTipeMotorDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.IsActive = grup.IsActive;
    }
}
