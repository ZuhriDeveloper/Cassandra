using Cassandra.Application.Contracts.CabangLeasing;
using Cassandra.Domain.CabangLeasing;
using Cassandra.Domain.CabangLeasing.Events;
using Cassandra.Domain.Common;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.CabangLeasing;

public class CabangLeasingRepository(AppDbContext context) : ICabangLeasingRepository
{
    private const string AggregateType = "CabangLeasing";

    public async Task<Domain.CabangLeasing.CabangLeasing?> GetByIdAsync(CabangLeasingId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.CabangLeasing.CabangLeasing.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.CabangLeasing.CabangLeasing cl, CancellationToken ct = default)
    {
        var newEvents = cl.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = cl.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = cl.DealerId,
                AggregateType = AggregateType,
                AggregateId   = cl.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(cl, newEvents, ct);
        await context.SaveChangesAsync(ct);
        cl.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.CabangLeasing.CabangLeasing cl,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.CabangLeasingReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == cl.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<CabangLeasingCreated>().First();
            projection = new CabangLeasingReadModel
            {
                Id        = cl.Id.Value,
                DealerId  = cl.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.CabangLeasingReadModels.Add(projection);
        }

        if (newEvents.OfType<CabangLeasingUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<CabangLeasingActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<CabangLeasingDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name           = cl.Name;
        projection.Phone          = cl.Phone;
        projection.Fax            = cl.Fax;
        projection.Contact        = cl.Contact;
        projection.GlobalLeasingId = cl.GlobalLeasingId;
        projection.IsActive       = cl.IsActive;
    }
}
