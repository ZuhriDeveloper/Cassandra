using Cassandra.Application.Contracts.PelanggaranWilayah;
using Cassandra.Domain.Common;
using Cassandra.Domain.PelanggaranWilayah;
using Cassandra.Domain.PelanggaranWilayah.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.PelanggaranWilayah;

public class PelanggaranWilayahRepository(AppDbContext context) : IPelanggaranWilayahRepository
{
    private const string AggregateType = "PelanggaranWilayah";

    public async Task<Domain.PelanggaranWilayah.PelanggaranWilayah?> GetByIdAsync(PelanggaranWilayahId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.PelanggaranWilayah.PelanggaranWilayah.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.PelanggaranWilayah.PelanggaranWilayah pw, CancellationToken ct = default)
    {
        var newEvents = pw.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = pw.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = pw.DealerId,
                AggregateType = AggregateType,
                AggregateId   = pw.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(pw, newEvents, ct);
        await context.SaveChangesAsync(ct);
        pw.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.PelanggaranWilayah.PelanggaranWilayah pw,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.PelanggaranWilayahReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == pw.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<PelanggaranWilayahCreated>().First();
            projection = new PelanggaranWilayahReadModel
            {
                Id        = pw.Id.Value,
                DealerId  = pw.DealerId,
                AreaCode  = created.AreaCode,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.PelanggaranWilayahReadModels.Add(projection);
        }

        if (newEvents.OfType<PelanggaranWilayahUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<PelanggaranWilayahActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<PelanggaranWilayahDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.ExtraFee = pw.ExtraFee;
        projection.IsActive = pw.IsActive;
    }
}
