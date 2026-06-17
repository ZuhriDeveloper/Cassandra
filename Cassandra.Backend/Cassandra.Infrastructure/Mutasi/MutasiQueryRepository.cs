using Cassandra.Application.Contracts.Mutasi;
using Cassandra.Application.DTOs.Mutasi;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Mutasi;

public class MutasiQueryRepository(AppDbContext context) : IMutasiQueryRepository
{
    public async Task<IReadOnlyList<MutasiDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.MutasiReadModels
            .AsNoTracking()
            .OrderByDescending(x => x.MutasiDate)
            .Select(x => new MutasiDto(
                x.Id, x.MutasiNumber, x.MutasiDate,
                x.SourceKiosId, x.DestinationKiosId, x.IsActive, null, null))
            .ToListAsync(ct);
    }

    public async Task<MutasiDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.MutasiReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : new MutasiDto(
            row.Id, row.MutasiNumber, row.MutasiDate,
            row.SourceKiosId, row.DestinationKiosId, row.IsActive, null, null);
    }

    public async Task<MutasiDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.MutasiReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        if (row is null) return null;

        var engineNumbers = await context.MutasiItemReadModels
            .AsNoTracking()
            .Where(x => x.MutasiId == id)
            .Select(x => x.NoMesin)
            .ToListAsync(ct);

        var kelengkapanItems = await context.MutasiKelengkapanReadModels
            .AsNoTracking()
            .Where(x => x.MutasiId == id)
            .Select(x => new MutasiKelengkapanItemDto(x.KelengkapanName, x.Qty))
            .ToListAsync(ct);

        return new MutasiDto(
            row.Id, row.MutasiNumber, row.MutasiDate,
            row.SourceKiosId, row.DestinationKiosId, row.IsActive,
            engineNumbers, kelengkapanItems);
    }

    public Task<bool> MutasiNumberExistsAsync(string mutasiNumber, CancellationToken ct = default)
        => context.MutasiReadModels.AnyAsync(x => x.MutasiNumber == mutasiNumber, ct);
}
