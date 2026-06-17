using Cassandra.Application.Contracts.Stock;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stock;
using Cassandra.Domain.Stock.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Stock;

public class StockRepository(AppDbContext context) : IStockRepository
{
    private const string AggregateType = "Stock";

    public async Task<Domain.Stock.Stock?> GetByIdAsync(StockId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Stock.Stock.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Stock.Stock stock, CancellationToken ct = default)
    {
        var newEvents = stock.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = stock.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = stock.DealerId,
                AggregateType = AggregateType,
                AggregateId   = stock.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(stock, newEvents, ct);
        await context.SaveChangesAsync(ct);
        stock.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Stock.Stock stock,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.StockReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == stock.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<StockCreated>().First();
            projection = new StockReadModel
            {
                Id            = stock.Id.Value,
                DealerId      = stock.DealerId,
                NoMesin       = created.NoMesin,
                NoRangka      = created.NoRangka,
                TipeMotorId   = created.TipeMotorId,
                WarnaId       = created.WarnaId,
                KiosId        = created.KiosId,
                SuratJalanId  = created.SuratJalanId,
                SuratJalanDate = created.SuratJalanDate,
                SoId          = created.SoId,
                AssemblyYear  = created.AssemblyYear,
                Status        = Domain.Stock.StockStatus.TERSEDIA,
                IsActive      = true,
                CreatedBy     = created.CreatedBy,
                CreatedAt     = created.OccurredAt,
            };
            context.StockReadModels.Add(projection);
        }

        if (newEvents.OfType<StockStatusChanged>().LastOrDefault() is { } statusChanged)
        {
            projection.Status    = statusChanged.Status;
            projection.UpdatedBy = statusChanged.UpdatedBy;
            projection.UpdatedAt = statusChanged.OccurredAt;
        }

        if (newEvents.OfType<StockMoved>().LastOrDefault() is { } moved)
        {
            projection.KiosId    = moved.NewKiosId;
            projection.UpdatedBy = moved.UpdatedBy;
            projection.UpdatedAt = moved.OccurredAt;
        }
    }
}
