using Cassandra.Application.Contracts.Jabatan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Jabatan;
using Cassandra.Domain.Jabatan.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Jabatan;

public class JabatanRepository(AppDbContext context) : IJabatanRepository
{
    private const string AggregateType = "Jabatan";

    public async Task<Domain.Jabatan.Jabatan?> GetByIdAsync(JabatanId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Jabatan.Jabatan.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Jabatan.Jabatan jabatan, CancellationToken ct = default)
    {
        var newEvents = jabatan.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = jabatan.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = jabatan.DealerId,
                AggregateType = AggregateType,
                AggregateId   = jabatan.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(jabatan, newEvents, ct);
        await context.SaveChangesAsync(ct);
        jabatan.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Jabatan.Jabatan jabatan,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.JabatanReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == jabatan.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<JabatanCreated>().First();
            projection = new JabatanReadModel
            {
                Id        = jabatan.Id.Value,
                DealerId  = jabatan.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.JabatanReadModels.Add(projection);
        }

        projection.Name        = jabatan.Name;
        projection.Description = jabatan.Description;
        projection.IsActive    = jabatan.IsActive;
    }
}
