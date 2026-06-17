using Cassandra.Application.Contracts.So;
using Cassandra.Application.DTOs.So;
using Cassandra.Domain.So;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.So;

public class SoQueryRepository(AppDbContext context) : ISoQueryRepository
{
    public async Task<IReadOnlyList<SoDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.SoReadModels
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.SoDate)
            .Select(x => new SoDto(
                x.Id, x.SoNumber, x.SoDate, x.DueDate, x.PaymentType,
                x.MetodeKeuanganId, x.QtyUnit, x.Total, x.Subsidi,
                x.CashDiscount, x.PPn, x.TotalAmount, x.Df, x.Status, null))
            .ToListAsync(ct);
    }

    public async Task<SoDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.SoReadModels
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : new SoDto(
            row.Id, row.SoNumber, row.SoDate, row.DueDate, row.PaymentType,
            row.MetodeKeuanganId, row.QtyUnit, row.Total, row.Subsidi,
            row.CashDiscount, row.PPn, row.TotalAmount, row.Df, row.Status, null);
    }

    public async Task<SoDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.SoReadModels
            .AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);

        if (row is null) return null;

        var items = await context.SoItemReadModels
            .AsNoTracking()
            .Where(x => x.SoId == id)
            .Select(x => new SoItemDto(x.TipeMotorId, x.WarnaId, x.Qty, x.NettPrice))
            .ToListAsync(ct);

        return new SoDto(
            row.Id, row.SoNumber, row.SoDate, row.DueDate, row.PaymentType,
            row.MetodeKeuanganId, row.QtyUnit, row.Total, row.Subsidi,
            row.CashDiscount, row.PPn, row.TotalAmount, row.Df, row.Status, items);
    }

    public Task<bool> SoNumberExistsAsync(string soNumber, CancellationToken ct = default)
        => context.SoReadModels.AnyAsync(x => x.SoNumber == soNumber, ct);

    public Task<bool> IsSoAktifAsync(Guid soId, CancellationToken ct = default)
        => context.SoReadModels.AnyAsync(x => x.Id == soId && x.Status == SoStatus.AKTIF && !x.IsDeleted, ct);
}
