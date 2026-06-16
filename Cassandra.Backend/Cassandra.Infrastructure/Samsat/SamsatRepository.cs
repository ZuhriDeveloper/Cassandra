using Cassandra.Application.Contracts.Samsat;
using Cassandra.Domain.Common;
using Cassandra.Domain.Samsat;
using Cassandra.Domain.Samsat.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Samsat;

public class SamsatRepository(AppDbContext context) : ISamsatRepository
{
    private const string AggregateType = "Samsat";

    public async Task<Domain.Samsat.Samsat?> GetByIdAsync(SamsatId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Samsat.Samsat.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Samsat.Samsat samsat, CancellationToken ct = default)
    {
        var newEvents = samsat.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = samsat.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = samsat.DealerId,
                AggregateType = AggregateType,
                AggregateId   = samsat.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(samsat, newEvents, ct);
        await context.SaveChangesAsync(ct);
        samsat.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Samsat.Samsat samsat,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.SamsatReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == samsat.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<SamsatCreated>().First();
            projection = new SamsatReadModel
            {
                Id        = samsat.Id.Value,
                DealerId  = samsat.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.SamsatReadModels.Add(projection);
        }

        if (newEvents.OfType<SamsatUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<SamsatActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<SamsatDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        if (newEvents.OfType<SamsatCitiesSet>().LastOrDefault() is { } citiesSet)
        {
            projection.UpdatedBy = citiesSet.UpdatedBy;
            projection.UpdatedAt = citiesSet.OccurredAt;

            var existingCities = await context.SamsatCityReadModels
                .Where(x => x.SamsatId == samsat.Id.Value)
                .ToListAsync(ct);
            context.SamsatCityReadModels.RemoveRange(existingCities);

            foreach (var city in citiesSet.Cities)
            {
                context.SamsatCityReadModels.Add(new SamsatCityReadModel
                {
                    SamsatId = samsat.Id.Value,
                    City     = city,
                });
            }
        }

        projection.Name     = samsat.Name;
        projection.IsActive = samsat.IsActive;
    }
}
