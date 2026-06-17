using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Domain.Common;
using Cassandra.Domain.RegistrasiPenjualan;
using Cassandra.Domain.RegistrasiPenjualan.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.RegistrasiPenjualan;

public class RegistrasiPenjualanRepository(AppDbContext context) : IRegistrasiPenjualanRepository
{
    private const string AggregateType = "RegistrasiPenjualan";

    public async Task<Domain.RegistrasiPenjualan.RegistrasiPenjualan?> GetByIdAsync(
        RegistrasiPenjualanId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.RegistrasiPenjualan.RegistrasiPenjualan.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.RegistrasiPenjualan.RegistrasiPenjualan registrasi, CancellationToken ct = default)
    {
        var newEvents = registrasi.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = registrasi.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = registrasi.DealerId,
                AggregateType = AggregateType,
                AggregateId   = registrasi.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(registrasi, newEvents, ct);
        await context.SaveChangesAsync(ct);
        registrasi.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.RegistrasiPenjualan.RegistrasiPenjualan registrasi,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.RegistrasiPenjualanReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == registrasi.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<RegistrasiPenjualanCreated>().First();
            projection = new RegistrasiPenjualanReadModel
            {
                Id                     = registrasi.Id.Value,
                DealerId               = registrasi.DealerId,
                NoPenjualan            = created.NoPenjualan,
                SaleDate               = created.SaleDate,
                KaryawanId             = created.KaryawanId,
                KiosId                 = created.KiosId,
                MediatorId             = created.MediatorId,
                MetodePenjualan        = created.MetodePenjualan,
                TipePenjualan          = created.TipePenjualan,
                NoMesin                = created.NoMesin,
                NoRangka               = created.NoRangka,
                NamaCustomer           = created.NamaCustomer,
                Address                = created.Address,
                Phone                  = created.Phone,
                Phone1                 = created.Phone1,
                Phone2                 = created.Phone2,
                OffRoad                = created.OffRoad,
                Bbn                    = created.Bbn,
                Discount               = created.Discount,
                ApprovedDiscount       = created.ApprovedDiscount,
                OriginalDiscount       = created.OriginalDiscount,
                Total                  = created.Total,
                AmbilUang              = created.AmbilUang,
                Dp                     = created.Dp,
                Angsuran               = created.Angsuran,
                Tac                    = created.Tac,
                DaftarHargaLeasingId   = created.DaftarHargaLeasingId,
                TenorCode              = created.TenorCode,
                TipeMotorCode          = created.TipeMotorCode,
                WarnaName              = created.WarnaName,
                SerahTerimaKendaraanId = created.SerahTerimaKendaraanId,
                TandaTerimaSementaraId = created.TandaTerimaSementaraId,
                Kelengkapan            = created.Kelengkapan,
                IsApproved             = created.IsApproved,
                IsSent                 = false,
                IsVoid                 = false,
                EnableToVoid           = false,
                CreatedBy              = created.CreatedBy,
                CreatedAt              = created.OccurredAt,
            };
            context.RegistrasiPenjualanReadModels.Add(projection);
        }

        if (newEvents.OfType<RegistrasiPenjualanApproved>().LastOrDefault() is { } approved)
        {
            projection.ApprovedDiscount = approved.ApprovedDiscount;
            projection.IsApproved       = true;
            projection.UpdatedBy        = approved.ApprovedBy;
            projection.UpdatedAt        = approved.OccurredAt;
        }

        if (newEvents.OfType<RegistrasiPenjualanSent>().LastOrDefault() is { } sent)
        {
            projection.IsSent    = true;
            projection.UpdatedBy = sent.SentBy;
            projection.UpdatedAt = sent.OccurredAt;
        }

        if (newEvents.OfType<RegistrasiPenjualanVoided>().LastOrDefault() is { } voided)
        {
            projection.IsVoid    = true;
            projection.UpdatedBy = voided.VoidedBy;
            projection.UpdatedAt = voided.OccurredAt;
        }

        if (newEvents.OfType<RegistrasiPenjualanEnableToVoidSet>().LastOrDefault() is { } enableToVoidSet)
        {
            projection.EnableToVoid = enableToVoidSet.EnableToVoid;
            projection.UpdatedBy    = enableToVoidSet.UpdatedBy;
            projection.UpdatedAt    = enableToVoidSet.OccurredAt;
        }
    }
}
