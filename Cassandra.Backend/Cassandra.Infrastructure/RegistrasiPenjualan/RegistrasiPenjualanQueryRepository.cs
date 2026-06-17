using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.DTOs.RegistrasiPenjualan;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.RegistrasiPenjualan;

public class RegistrasiPenjualanQueryRepository(AppDbContext context) : IRegistrasiPenjualanQueryRepository
{
    public async Task<IReadOnlyList<RegistrasiPenjualanDto>> GetAllAsync(CancellationToken ct = default)
        => await context.RegistrasiPenjualanReadModels
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

    public async Task<RegistrasiPenjualanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.RegistrasiPenjualanReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : ToDto(row);
    }

    public Task<bool> NoPenjualanExistsAsync(string noPenjualan, CancellationToken ct = default)
        => context.RegistrasiPenjualanReadModels
            .AnyAsync(x => x.NoPenjualan == noPenjualan, ct);

    private static RegistrasiPenjualanDto ToDto(Persistence.Projections.RegistrasiPenjualanReadModel x)
    {
        var kelengkapan = string.IsNullOrEmpty(x.Kelengkapan)
            ? new List<string>()
            : x.Kelengkapan.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        var status = x.IsVoid       ? "VOID"
                   : x.IsSent       ? "SENT"
                   : x.IsApproved   ? "APPROVED"
                   : "PENDING";

        return new RegistrasiPenjualanDto(
            x.Id,
            x.NoPenjualan,
            x.SaleDate,
            x.KaryawanId,
            x.KiosId,
            x.MediatorId,
            x.MetodePenjualan,
            x.TipePenjualan,
            x.NoMesin,
            x.NoRangka,
            x.NamaCustomer,
            x.Address,
            x.Phone,
            x.Phone1,
            x.Phone2,
            x.OffRoad,
            x.Bbn,
            x.Discount,
            x.ApprovedDiscount,
            x.OriginalDiscount,
            x.Total,
            x.AmbilUang,
            x.Dp,
            x.Angsuran,
            x.Tac,
            x.DaftarHargaLeasingId,
            x.TenorCode,
            x.TipeMotorCode,
            x.WarnaName,
            x.SerahTerimaKendaraanId,
            x.TandaTerimaSementaraId,
            kelengkapan,
            x.IsApproved,
            x.IsSent,
            x.IsVoid,
            x.EnableToVoid,
            status);
    }
}
