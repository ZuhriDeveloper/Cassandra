using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;
using Cassandra.Domain.Karyawan.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Karyawan;

public class KaryawanRepository(AppDbContext context) : IKaryawanRepository
{
    private const string AggregateType = "Karyawan";

    public async Task<Domain.Karyawan.Karyawan?> GetByIdAsync(KaryawanId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Karyawan.Karyawan.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Karyawan.Karyawan karyawan, CancellationToken ct = default)
    {
        var newEvents = karyawan.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = karyawan.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = karyawan.DealerId,
                AggregateType = AggregateType,
                AggregateId   = karyawan.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(karyawan, newEvents, ct);
        await context.SaveChangesAsync(ct);
        karyawan.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Karyawan.Karyawan karyawan,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.KaryawanReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == karyawan.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<KaryawanCreated>().First();
            projection = new KaryawanReadModel
            {
                Id        = karyawan.Id.Value,
                DealerId  = karyawan.DealerId,
                KtpNumber = created.KtpNumber,
                Gender    = created.Gender.ToString(),
                HireDate  = created.HireDate,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.KaryawanReadModels.Add(projection);
        }

        projection.Name       = karyawan.Name;
        projection.Email      = karyawan.Email;
        projection.Phone      = karyawan.Phone;
        projection.PhoneAlt   = karyawan.PhoneAlt;
        projection.Address    = karyawan.Address;
        projection.SalesLimit = karyawan.SalesLimit;
        projection.JabatanId  = karyawan.JabatanId;
        projection.IsActive   = karyawan.IsActive;
        projection.ResignDate = karyawan.ResignDate;
    }
}
