using Cassandra.Application.Contracts.SoPenerimaan;
using Cassandra.Domain.Common;
using Cassandra.Domain.SoPenerimaan;
using Cassandra.Domain.SoPenerimaan.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.SoPenerimaan;

public class SoPenerimaanRepository(AppDbContext context) : ISoPenerimaanRepository
{
    private const string AggregateType = "SoPenerimaan";

    public async Task<Domain.SoPenerimaan.SoPenerimaan?> GetByIdAsync(SoPenerimaanId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.SoPenerimaan.SoPenerimaan.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.SoPenerimaan.SoPenerimaan soPenerimaan, CancellationToken ct = default)
    {
        var newEvents = soPenerimaan.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = soPenerimaan.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = soPenerimaan.DealerId,
                AggregateType = AggregateType,
                AggregateId   = soPenerimaan.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(soPenerimaan, newEvents, ct);
        await context.SaveChangesAsync(ct);
        soPenerimaan.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.SoPenerimaan.SoPenerimaan soPenerimaan,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.SoPenerimaanReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == soPenerimaan.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<SoPenerimaanCreated>().First();
            projection = new SoPenerimaanReadModel
            {
                Id            = soPenerimaan.Id.Value,
                DealerId      = soPenerimaan.DealerId,
                SuratJalanId  = created.SuratJalanId,
                SuratJalanDate = created.SuratJalanDate,
                SoId          = created.SoId,
                CreatedBy     = created.CreatedBy,
                CreatedAt     = created.OccurredAt,
            };
            context.SoPenerimaanReadModels.Add(projection);

            // Delete and re-insert motor items
            var existingMotorItems = await context.SoPenerimaanItemMotorReadModels
                .Where(x => x.SoPenerimaanId == soPenerimaan.Id.Value)
                .ToListAsync(ct);
            context.SoPenerimaanItemMotorReadModels.RemoveRange(existingMotorItems);

            foreach (var item in created.MotorItems)
            {
                context.SoPenerimaanItemMotorReadModels.Add(new SoPenerimaanItemMotorReadModel
                {
                    SoPenerimaanId = soPenerimaan.Id.Value,
                    TipeMotorId    = item.TipeMotorId,
                    WarnaId        = item.WarnaId,
                    NoMesin        = item.NoMesin,
                    NoRangka       = item.NoRangka,
                    KiosId         = item.KiosId,
                    AssemblyYear   = item.AssemblyYear,
                });
            }

            // Delete and re-insert kelengkapan items
            var existingKelengkapanItems = await context.SoPenerimaanItemKelengkapanReadModels
                .Where(x => x.SoPenerimaanId == soPenerimaan.Id.Value)
                .ToListAsync(ct);
            context.SoPenerimaanItemKelengkapanReadModels.RemoveRange(existingKelengkapanItems);

            foreach (var item in created.KelengkapanItems)
            {
                context.SoPenerimaanItemKelengkapanReadModels.Add(new SoPenerimaanItemKelengkapanReadModel
                {
                    SoPenerimaanId = soPenerimaan.Id.Value,
                    KelengkapanId  = item.KelengkapanId,
                    Qty            = item.Qty,
                    Notes          = item.Notes,
                });
            }
        }
    }
}
