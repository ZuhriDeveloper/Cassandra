using Cassandra.Application.Contracts.GlobalLeasing;
using Cassandra.Domain.Common;
using Cassandra.Domain.GlobalLeasing;
using Cassandra.Domain.GlobalLeasing.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.GlobalLeasing;

public class GlobalLeasingRepository(AppDbContext context) : IGlobalLeasingRepository
{
    private const string AggregateType = "GlobalLeasing";

    public async Task<Domain.GlobalLeasing.GlobalLeasing?> GetByIdAsync(GlobalLeasingId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.GlobalLeasing.GlobalLeasing.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.GlobalLeasing.GlobalLeasing gl, CancellationToken ct = default)
    {
        var newEvents = gl.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = gl.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = gl.DealerId,
                AggregateType = AggregateType,
                AggregateId   = gl.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(gl, newEvents, ct);
        await context.SaveChangesAsync(ct);
        gl.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.GlobalLeasing.GlobalLeasing gl,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.GlobalLeasingReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == gl.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<GlobalLeasingCreated>().First();
            projection = new GlobalLeasingReadModel
            {
                Id        = gl.Id.Value,
                DealerId  = gl.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.GlobalLeasingReadModels.Add(projection);
        }

        if (newEvents.OfType<GlobalLeasingUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<GlobalLeasingActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<GlobalLeasingDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name     = gl.Name;
        projection.Phone    = gl.Phone;
        projection.Fax      = gl.Fax;
        projection.Contact  = gl.Contact;
        projection.Address  = gl.Address;
        projection.IsActive = gl.IsActive;
    }
}
