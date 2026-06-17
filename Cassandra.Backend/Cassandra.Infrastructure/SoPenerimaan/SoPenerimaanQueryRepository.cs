using Cassandra.Application.Contracts.SoPenerimaan;
using Cassandra.Application.DTOs.SoPenerimaan;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.SoPenerimaan;

public class SoPenerimaanQueryRepository(AppDbContext context) : ISoPenerimaanQueryRepository
{
    public async Task<IReadOnlyList<SoPenerimaanDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.SoPenerimaanReadModels
            .AsNoTracking()
            .OrderByDescending(x => x.SuratJalanDate)
            .Select(x => new SoPenerimaanDto(x.Id, x.SuratJalanId, x.SuratJalanDate, x.SoId, null, null))
            .ToListAsync(ct);
    }

    public async Task<SoPenerimaanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.SoPenerimaanReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : new SoPenerimaanDto(row.Id, row.SuratJalanId, row.SuratJalanDate, row.SoId, null, null);
    }

    public async Task<SoPenerimaanDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.SoPenerimaanReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        if (row is null) return null;

        var motorItems = await context.SoPenerimaanItemMotorReadModels
            .AsNoTracking()
            .Where(x => x.SoPenerimaanId == id)
            .Select(x => new SoPenerimaanItemMotorDto(
                x.TipeMotorId, x.WarnaId, x.NoMesin, x.NoRangka, x.KiosId, x.AssemblyYear))
            .ToListAsync(ct);

        var kelengkapanItems = await context.SoPenerimaanItemKelengkapanReadModels
            .AsNoTracking()
            .Where(x => x.SoPenerimaanId == id)
            .Select(x => new SoPenerimaanItemKelengkapanDto(x.KelengkapanId, x.Qty, x.Notes))
            .ToListAsync(ct);

        return new SoPenerimaanDto(row.Id, row.SuratJalanId, row.SuratJalanDate, row.SoId, motorItems, kelengkapanItems);
    }

    public Task<bool> SuratJalanIdExistsAsync(string suratJalanId, CancellationToken ct = default)
        => context.SoPenerimaanReadModels.AnyAsync(x => x.SuratJalanId == suratJalanId, ct);

    public Task<bool> HasPenerimaanForSoAsync(Guid soId, CancellationToken ct = default)
        => context.SoPenerimaanReadModels.AnyAsync(x => x.SoId == soId, ct);
}
