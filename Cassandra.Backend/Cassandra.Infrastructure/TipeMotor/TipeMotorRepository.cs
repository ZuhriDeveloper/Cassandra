using Cassandra.Application.Contracts.TipeMotor;
using Cassandra.Domain.Common;
using Cassandra.Domain.TipeMotor;
using Cassandra.Domain.TipeMotor.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.TipeMotor;

public class TipeMotorRepository(AppDbContext context) : ITipeMotorRepository
{
    private const string AggregateType = "TipeMotor";

    public async Task<Domain.TipeMotor.TipeMotor?> GetByIdAsync(TipeMotorId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.TipeMotor.TipeMotor.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.TipeMotor.TipeMotor tipe, CancellationToken ct = default)
    {
        var newEvents = tipe.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = tipe.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = tipe.DealerId,
                AggregateType = AggregateType,
                AggregateId   = tipe.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(tipe, newEvents, ct);
        await context.SaveChangesAsync(ct);
        tipe.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.TipeMotor.TipeMotor tipe,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.TipeMotorReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == tipe.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<TipeMotorCreated>().First();
            projection = new TipeMotorReadModel
            {
                Id        = tipe.Id.Value,
                DealerId  = tipe.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.TipeMotorReadModels.Add(projection);
        }

        if (newEvents.OfType<TipeMotorUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<TipeMotorActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<TipeMotorDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        if (newEvents.OfType<TipeMotorColorsSet>().LastOrDefault() is { } colorsSet)
        {
            projection.UpdatedBy = colorsSet.UpdatedBy;
            projection.UpdatedAt = colorsSet.OccurredAt;

            // Replace all color rows for this TipeMotor
            var existingColors = await context.TipeMotorWarnaReadModels
                .Where(x => x.TipeMotorId == tipe.Id.Value)
                .ToListAsync(ct);
            context.TipeMotorWarnaReadModels.RemoveRange(existingColors);

            foreach (var warnaId in colorsSet.WarnaIds)
            {
                context.TipeMotorWarnaReadModels.Add(new TipeMotorWarnaReadModel
                {
                    TipeMotorId = tipe.Id.Value,
                    WarnaId     = warnaId,
                });
            }
        }

        projection.GrupTipeMotorId     = tipe.GrupTipeMotorId;
        projection.ShortName            = tipe.ShortName;
        projection.ProductCode          = tipe.ProductCode;
        projection.WmsCode              = tipe.WmsCode;
        projection.AhmCode              = tipe.AhmCode;
        projection.EngineNumberFormat   = tipe.EngineNumberFormat;
        projection.ChassisNumberFormat  = tipe.ChassisNumberFormat;
        projection.NettPrice            = tipe.NettPrice;
        projection.OrJakarta            = tipe.OrJakarta;
        projection.OrTangerang          = tipe.OrTangerang;
        projection.BbnJakarta           = tipe.BbnJakarta;
        projection.BbnTangerang         = tipe.BbnTangerang;
        projection.IsActive             = tipe.IsActive;
    }
}
