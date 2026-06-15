using Cassandra.Application.Contracts.Warna;
using Cassandra.Domain.Common;
using Cassandra.Domain.Warna;
using Cassandra.Domain.Warna.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Warna;

public class WarnaRepository(AppDbContext context) : IWarnaRepository
{
    private const string AggregateType = "Warna";

    public async Task<Domain.Warna.Warna?> GetByIdAsync(WarnaId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Warna.Warna.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Warna.Warna warna, CancellationToken ct = default)
    {
        var newEvents = warna.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = warna.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = warna.DealerId,
                AggregateType = AggregateType,
                AggregateId   = warna.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(warna, newEvents, ct);
        await context.SaveChangesAsync(ct);
        warna.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Warna.Warna warna,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.WarnaReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == warna.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<WarnaCreated>().First();
            projection = new WarnaReadModel
            {
                Id        = warna.Id.Value,
                DealerId  = warna.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.WarnaReadModels.Add(projection);
        }

        if (newEvents.OfType<WarnaUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<WarnaActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<WarnaDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name     = warna.Name;
        projection.IsActive = warna.IsActive;
    }
}
