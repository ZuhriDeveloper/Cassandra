using Cassandra.Application.Contracts.PengirimanMotor;
using Cassandra.Domain.Common;
using Cassandra.Domain.PengirimanMotor;
using Cassandra.Domain.PengirimanMotor.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.PengirimanMotor;

public class PengirimanMotorRepository(AppDbContext context) : IPengirimanMotorRepository
{
    private const string AggregateType = "PengirimanMotor";

    public async Task<Domain.PengirimanMotor.PengirimanMotor?> GetByIdAsync(
        PengirimanMotorId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.PengirimanMotor.PengirimanMotor.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.PengirimanMotor.PengirimanMotor pengiriman, CancellationToken ct = default)
    {
        var newEvents = pengiriman.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = pengiriman.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = pengiriman.DealerId,
                AggregateType = AggregateType,
                AggregateId   = pengiriman.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(pengiriman, newEvents, ct);
        await context.SaveChangesAsync(ct);
        pengiriman.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.PengirimanMotor.PengirimanMotor pengiriman,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.PengirimanMotorReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == pengiriman.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<PengirimanMotorCreated>().First();
            projection = new PengirimanMotorReadModel
            {
                Id                    = pengiriman.Id.Value,
                DealerId              = pengiriman.DealerId,
                RegistrasiPenjualanId = created.RegistrasiPenjualanId,
                NoMesin               = created.NoMesin,
                Driver1Id             = created.Driver1Id,
                Driver2Id             = created.Driver2Id,
                DeliveryDate          = created.DeliveryDate,
                Zona                  = created.Zona,
                CreatedBy             = created.CreatedBy,
                CreatedAt             = created.OccurredAt,
            };
            context.PengirimanMotorReadModels.Add(projection);
        }
    }
}
