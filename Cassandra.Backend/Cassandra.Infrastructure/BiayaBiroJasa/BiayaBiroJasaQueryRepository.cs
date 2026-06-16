using Cassandra.Application.Contracts.BiayaBiroJasa;
using Cassandra.Application.DTOs.BiayaBiroJasa;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.BiayaBiroJasa;

public class BiayaBiroJasaQueryRepository(AppDbContext context) : IBiayaBiroJasaQueryRepository
{
    public async Task<BiayaBiroJasaDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var header = await context.BiayaBiroJasaReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        if (header is null) return null;

        var items = await context.BiayaBiroJasaItemReadModels
            .AsNoTracking()
            .Where(x => x.BiayaBiroJasaId == id)
            .Select(x => new BiayaBiroJasaItemDto(x.TipeMotorId, x.BiayaStnk, x.Notice))
            .ToListAsync(ct);

        return new BiayaBiroJasaDto(header.Id, header.SamsatId, header.BiroId, header.IsActive, items);
    }

    public async Task<IReadOnlyList<BiayaBiroJasaDto>> GetAllAsync(CancellationToken ct = default)
    {
        var headers = await context.BiayaBiroJasaReadModels
            .AsNoTracking()
            .OrderBy(x => x.SamsatId)
            .ThenBy(x => x.BiroId)
            .ToListAsync(ct);

        if (headers.Count == 0) return [];

        var ids = headers.Select(h => h.Id).ToList();
        var allItems = await context.BiayaBiroJasaItemReadModels
            .AsNoTracking()
            .Where(x => ids.Contains(x.BiayaBiroJasaId))
            .ToListAsync(ct);

        return headers.Select(h =>
        {
            var items = allItems
                .Where(x => x.BiayaBiroJasaId == h.Id)
                .Select(x => new BiayaBiroJasaItemDto(x.TipeMotorId, x.BiayaStnk, x.Notice))
                .ToList();
            return new BiayaBiroJasaDto(h.Id, h.SamsatId, h.BiroId, h.IsActive, items);
        }).ToList();
    }

    public Task<bool> SamsatBiroExistsAsync(Guid samsatId, Guid biroId, CancellationToken ct = default)
        => context.BiayaBiroJasaReadModels.AnyAsync(x => x.SamsatId == samsatId && x.BiroId == biroId, ct);
}
