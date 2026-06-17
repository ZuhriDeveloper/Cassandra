using Cassandra.Application.Contracts.Bpkb;
using Cassandra.Domain.Bpkb;
using Cassandra.Domain.Bpkb.Events;
using Cassandra.Domain.Common;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Bpkb;

public class BpkbRepository(AppDbContext context) : IBpkbRepository
{
    private const string AggregateType = "Bpkb";

    public async Task<Domain.Bpkb.Bpkb?> GetByIdAsync(BpkbId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Bpkb.Bpkb.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Bpkb.Bpkb bpkb, CancellationToken ct = default)
    {
        var newEvents = bpkb.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = bpkb.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = bpkb.DealerId,
                AggregateType = AggregateType,
                AggregateId   = bpkb.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(bpkb, newEvents, ct);
        await context.SaveChangesAsync(ct);
        bpkb.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Bpkb.Bpkb bpkb,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.BpkbReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == bpkb.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<BpkbCreated>().First();
            projection = new BpkbReadModel
            {
                Id        = bpkb.Id.Value,
                DealerId  = bpkb.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.BpkbReadModels.Add(projection);
        }

        projection.RegistrasiPenjualanId = bpkb.RegistrasiPenjualanId;
        projection.StnkId                = bpkb.StnkId;
        projection.Status                = bpkb.Status;
        projection.RequestDate           = bpkb.RequestDate;
        projection.BpkbNumber            = bpkb.BpkbNumber;
        projection.BookNumber            = bpkb.BookNumber;
        projection.ReceiveDate           = bpkb.ReceiveDate;
        projection.HandoverDate          = bpkb.HandoverDate;
        projection.Receiver              = bpkb.Receiver;

        if (newEvents.Any(e => e is not BpkbCreated))
            projection.UpdatedAt = DateTime.UtcNow;
    }
}
