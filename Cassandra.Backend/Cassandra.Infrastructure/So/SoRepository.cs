using Cassandra.Application.Contracts.So;
using Cassandra.Domain.Common;
using Cassandra.Domain.So;
using Cassandra.Domain.So.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.So;

public class SoRepository(AppDbContext context) : ISoRepository
{
    private const string AggregateType = "So";

    public async Task<Domain.So.So?> GetByIdAsync(SoId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.So.So.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.So.So so, CancellationToken ct = default)
    {
        var newEvents = so.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = so.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = so.DealerId,
                AggregateType = AggregateType,
                AggregateId   = so.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(so, newEvents, ct);
        await context.SaveChangesAsync(ct);
        so.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.So.So so,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.SoReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == so.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<SoCreated>().First();
            projection = new SoReadModel
            {
                Id              = so.Id.Value,
                DealerId        = so.DealerId,
                SoNumber        = created.SoNumber,
                SoDate          = created.SoDate,
                DueDate         = created.DueDate,
                PaymentType     = created.PaymentType,
                MetodeKeuanganId = created.MetodeKeuanganId,
                QtyUnit         = created.QtyUnit,
                Total           = created.Total,
                Subsidi         = created.Subsidi,
                CashDiscount    = created.CashDiscount,
                PPn             = created.PPn,
                TotalAmount     = created.TotalAmount,
                Df              = created.Df,
                Status          = Domain.So.SoStatus.AKTIF,
                IsDeleted       = false,
                CreatedBy       = created.CreatedBy,
                CreatedAt       = created.OccurredAt,
            };
            context.SoReadModels.Add(projection);

            // Insert items
            foreach (var item in created.Items)
            {
                context.SoItemReadModels.Add(new SoItemReadModel
                {
                    SoId        = so.Id.Value,
                    TipeMotorId = item.TipeMotorId,
                    WarnaId     = item.WarnaId,
                    Qty         = item.Qty,
                    NettPrice   = item.NettPrice,
                });
            }
        }

        if (newEvents.OfType<SoStatusChanged>().LastOrDefault() is { } statusChanged)
        {
            projection.Status    = statusChanged.Status;
            projection.UpdatedBy = statusChanged.UpdatedBy;
            projection.UpdatedAt = statusChanged.OccurredAt;
        }

        if (newEvents.OfType<SoDeleted>().LastOrDefault() is { } deleted)
        {
            projection.IsDeleted = true;
            projection.UpdatedBy = deleted.DeletedBy;
            projection.UpdatedAt = deleted.OccurredAt;
        }
    }
}
