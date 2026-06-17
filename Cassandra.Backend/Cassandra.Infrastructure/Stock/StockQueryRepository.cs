using Cassandra.Application.Contracts.Stock;
using Cassandra.Application.DTOs.Stock;
using Cassandra.Domain.Stock;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Stock;

public class StockQueryRepository(AppDbContext context) : IStockQueryRepository
{
    public async Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.StockReadModels
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new StockDto(
                x.Id, x.NoMesin, x.NoRangka, x.TipeMotorId, x.WarnaId,
                x.KiosId, x.SuratJalanId, x.SuratJalanDate, x.SoId,
                x.AssemblyYear, x.Status, x.IsActive))
            .ToListAsync(ct);
    }

    public async Task<StockDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.StockReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : new StockDto(
            row.Id, row.NoMesin, row.NoRangka, row.TipeMotorId, row.WarnaId,
            row.KiosId, row.SuratJalanId, row.SuratJalanDate, row.SoId,
            row.AssemblyYear, row.Status, row.IsActive);
    }

    public async Task<StockDto?> GetByNoMesinAsync(string noMesin, CancellationToken ct = default)
    {
        var row = await context.StockReadModels
            .AsNoTracking()
            .Where(x => x.NoMesin == noMesin)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : new StockDto(
            row.Id, row.NoMesin, row.NoRangka, row.TipeMotorId, row.WarnaId,
            row.KiosId, row.SuratJalanId, row.SuratJalanDate, row.SoId,
            row.AssemblyYear, row.Status, row.IsActive);
    }

    public Task<bool> NoMesinExistsAsync(string noMesin, CancellationToken ct = default)
        => context.StockReadModels.AnyAsync(x => x.NoMesin == noMesin, ct);

    public async Task<IReadOnlyList<StockDto>> GetAvailableForKiosAsync(Guid kiosId, CancellationToken ct = default)
    {
        return await context.StockReadModels
            .AsNoTracking()
            .Where(x => x.KiosId == kiosId && x.Status == StockStatus.TERSEDIA && x.IsActive)
            .Select(x => new StockDto(
                x.Id, x.NoMesin, x.NoRangka, x.TipeMotorId, x.WarnaId,
                x.KiosId, x.SuratJalanId, x.SuratJalanDate, x.SoId,
                x.AssemblyYear, x.Status, x.IsActive))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<StockDto>> GetAllByTipeMotorAsync(Guid tipeMotorId, CancellationToken ct = default)
    {
        return await context.StockReadModels
            .AsNoTracking()
            .Where(x => x.TipeMotorId == tipeMotorId)
            .Select(x => new StockDto(
                x.Id, x.NoMesin, x.NoRangka, x.TipeMotorId, x.WarnaId,
                x.KiosId, x.SuratJalanId, x.SuratJalanDate, x.SoId,
                x.AssemblyYear, x.Status, x.IsActive))
            .ToListAsync(ct);
    }
}
