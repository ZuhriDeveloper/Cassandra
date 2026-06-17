using Cassandra.Application.Contracts.Stnk;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;
using Cassandra.Domain.Stnk.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Stnk;

public class StnkRepository(AppDbContext context) : IStnkRepository
{
    private const string AggregateType = "Stnk";

    public async Task<Domain.Stnk.Stnk?> GetByIdAsync(StnkId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.Stnk.Stnk.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.Stnk.Stnk stnk, CancellationToken ct = default)
    {
        var newEvents = stnk.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = stnk.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = stnk.DealerId,
                AggregateType = AggregateType,
                AggregateId   = stnk.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(stnk, newEvents, ct);
        await context.SaveChangesAsync(ct);
        stnk.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.Stnk.Stnk stnk,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.StnkReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == stnk.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<StnkCreated>().First();
            projection = new StnkReadModel
            {
                Id        = stnk.Id.Value,
                DealerId  = stnk.DealerId,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.StnkReadModels.Add(projection);
        }

        projection.RegistrasiPenjualanId = stnk.RegistrasiPenjualanId;
        projection.Status                = stnk.Status;
        projection.FakturDate            = stnk.FakturDate;
        projection.FakturName            = stnk.FakturName;
        projection.FakturAddress         = stnk.FakturAddress;
        projection.ProcessDate           = stnk.ProcessDate;
        projection.BiroId                = stnk.BiroId;
        projection.InvoiceNumber         = stnk.InvoiceNumber;
        projection.PlateNumber           = stnk.PlateNumber;
        projection.StnkNumber            = stnk.StnkNumber;
        projection.StnkCost              = stnk.StnkCost;
        projection.ProgressiveCost       = stnk.ProgressiveCost;
        projection.NoticeCost            = stnk.NoticeCost;
        projection.ReceiveDate           = stnk.ReceiveDate;
        projection.HandoverDate          = stnk.HandoverDate;
        projection.StnkReceiver          = stnk.StnkReceiver;
        projection.Region                = stnk.Region;
        projection.BbnCost               = stnk.BbnCost;
        projection.PnbpCost              = stnk.PnbpCost;
        projection.AdminCost             = stnk.AdminCost;
        projection.OtherCost             = stnk.OtherCost;
        projection.ServiceCost           = stnk.ServiceCost;
        projection.PphCost               = stnk.PphCost;
        projection.IsInvoiceValid        = stnk.IsInvoiceValid;

        if (newEvents.Any(e => e is not StnkCreated))
            projection.UpdatedAt = DateTime.UtcNow;
    }
}
