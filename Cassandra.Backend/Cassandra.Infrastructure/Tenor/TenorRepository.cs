using Cassandra.Application.Contracts.Tenor;
using Cassandra.Domain.Common;
using Cassandra.Domain.Tenor;
using Cassandra.Domain.Tenor.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Tenor;

public class TenorRepository(AppDbContext context) : ITenorRepository
{
    private const string AggregateType = "Tenor";

    public async Task<Domain.Tenor.Tenor?> GetByIdAsync(TenorId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Tenor.Tenor.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Tenor.Tenor tenor, CancellationToken ct = default)
    {
        var newEvents = tenor.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = tenor.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = tenor.DealerId,
                AggregateType = AggregateType,
                AggregateId   = tenor.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(tenor, newEvents, ct);
        await context.SaveChangesAsync(ct);
        tenor.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Tenor.Tenor tenor,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.TenorReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == tenor.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<TenorCreated>().First();
            projection = new TenorReadModel
            {
                Id        = tenor.Id.Value,
                DealerId  = tenor.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.TenorReadModels.Add(projection);
        }

        if (newEvents.OfType<TenorUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<TenorActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<TenorDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name        = tenor.Name;
        projection.Months      = tenor.Months;
        projection.GrupTenorId = tenor.GrupTenorId;
        projection.IsActive    = tenor.IsActive;
    }
}
