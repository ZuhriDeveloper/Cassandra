using Cassandra.Application.Contracts.Mutasi;
using Cassandra.Domain.Common;
using Cassandra.Domain.Mutasi;
using Cassandra.Domain.Mutasi.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Mutasi;

public class MutasiRepository(AppDbContext context) : IMutasiRepository
{
    private const string AggregateType = "Mutasi";

    public async Task<Domain.Mutasi.Mutasi?> GetByIdAsync(MutasiId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Mutasi.Mutasi.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Mutasi.Mutasi mutasi, CancellationToken ct = default)
    {
        var newEvents = mutasi.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = mutasi.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = mutasi.DealerId,
                AggregateType = AggregateType,
                AggregateId   = mutasi.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(mutasi, newEvents, ct);
        await context.SaveChangesAsync(ct);
        mutasi.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Mutasi.Mutasi mutasi,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.MutasiReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == mutasi.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<MutasiCreated>().First();
            projection = new MutasiReadModel
            {
                Id                = mutasi.Id.Value,
                DealerId          = mutasi.DealerId,
                MutasiNumber      = created.MutasiNumber,
                MutasiDate        = created.MutasiDate,
                SourceKiosId      = created.SourceKiosId,
                DestinationKiosId = created.DestinationKiosId,
                IsActive          = true,
                CreatedBy         = created.CreatedBy,
                CreatedAt         = created.OccurredAt,
            };
            context.MutasiReadModels.Add(projection);

            // Insert engine number items
            var existingItems = await context.MutasiItemReadModels
                .Where(x => x.MutasiId == mutasi.Id.Value)
                .ToListAsync(ct);
            context.MutasiItemReadModels.RemoveRange(existingItems);

            foreach (var noMesin in created.EngineNumbers)
            {
                context.MutasiItemReadModels.Add(new MutasiItemReadModel
                {
                    MutasiId = mutasi.Id.Value,
                    NoMesin  = noMesin,
                });
            }

            // Insert kelengkapan items
            var existingKelengkapan = await context.MutasiKelengkapanReadModels
                .Where(x => x.MutasiId == mutasi.Id.Value)
                .ToListAsync(ct);
            context.MutasiKelengkapanReadModels.RemoveRange(existingKelengkapan);

            foreach (var item in created.KelengkapanItems)
            {
                context.MutasiKelengkapanReadModels.Add(new MutasiKelengkapanReadModel
                {
                    MutasiId       = mutasi.Id.Value,
                    KelengkapanName = item.KelengkapanName,
                    Qty            = item.Qty,
                });
            }
        }
    }
}
