using Cassandra.Application.Contracts.SoRetur;
using Cassandra.Domain.Common;
using Cassandra.Domain.SoRetur;
using Cassandra.Domain.SoRetur.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.SoRetur;

public class SoReturRepository(AppDbContext context) : ISoReturRepository
{
    private const string AggregateType = "SoRetur";

    public async Task<Domain.SoRetur.SoRetur?> GetByIdAsync(SoReturId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.SoRetur.SoRetur.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.SoRetur.SoRetur soRetur, CancellationToken ct = default)
    {
        var newEvents = soRetur.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = soRetur.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = soRetur.DealerId,
                AggregateType = AggregateType,
                AggregateId   = soRetur.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(soRetur, newEvents, ct);
        await context.SaveChangesAsync(ct);
        soRetur.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.SoRetur.SoRetur soRetur,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.SoReturReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == soRetur.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<SoReturCreated>().First();
            projection = new SoReturReadModel
            {
                Id          = soRetur.Id.Value,
                DealerId    = soRetur.DealerId,
                ReturNumber = created.ReturNumber,
                SoId        = created.SoId,
                ReturDate   = created.ReturDate,
                Reason      = created.Reason,
                Total       = created.Total,
                PPn         = created.PPn,
                TotalAmount = created.TotalAmount,
                CreatedBy   = created.CreatedBy,
                CreatedAt   = created.OccurredAt,
            };
            context.SoReturReadModels.Add(projection);

            // Insert items
            var existingItems = await context.SoReturItemReadModels
                .Where(x => x.SoReturId == soRetur.Id.Value)
                .ToListAsync(ct);
            context.SoReturItemReadModels.RemoveRange(existingItems);

            foreach (var item in created.Items)
            {
                context.SoReturItemReadModels.Add(new SoReturItemReadModel
                {
                    SoReturId   = soRetur.Id.Value,
                    TipeMotorId = item.TipeMotorId,
                    WarnaId     = item.WarnaId,
                    Qty         = item.Qty,
                    NettPrice   = item.NettPrice,
                });
            }
        }
    }
}
