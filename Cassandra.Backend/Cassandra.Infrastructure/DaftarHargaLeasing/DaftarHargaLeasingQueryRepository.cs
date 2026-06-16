using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Application.DTOs.DaftarHargaLeasing;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.DaftarHargaLeasing;

public class DaftarHargaLeasingQueryRepository(AppDbContext context) : IDaftarHargaLeasingQueryRepository
{
    public async Task<DaftarHargaLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var header = await context.DaftarHargaLeasingReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        if (header is null) return null;

        var items = await context.DaftarHargaLeasingItemReadModels
            .AsNoTracking()
            .Where(x => x.DaftarHargaLeasingId == id)
            .Select(x => new DaftarHargaLeasingItemDto(x.GrupTipeMotorId, x.Subsidi, x.Incentive, x.LainLain, x.Subsidi + x.Incentive + x.LainLain))
            .ToListAsync(ct);

        return new DaftarHargaLeasingDto(header.Id, header.Name, header.GlobalLeasingId, header.GrupTenorId, header.IsActive, items);
    }

    public async Task<IReadOnlyList<DaftarHargaLeasingDto>> GetAllAsync(CancellationToken ct = default)
    {
        var headers = await context.DaftarHargaLeasingReadModels
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        if (headers.Count == 0) return [];

        var ids = headers.Select(h => h.Id).ToList();
        var allItems = await context.DaftarHargaLeasingItemReadModels
            .AsNoTracking()
            .Where(x => ids.Contains(x.DaftarHargaLeasingId))
            .ToListAsync(ct);

        return headers.Select(h =>
        {
            var items = allItems
                .Where(x => x.DaftarHargaLeasingId == h.Id)
                .Select(x => new DaftarHargaLeasingItemDto(x.GrupTipeMotorId, x.Subsidi, x.Incentive, x.LainLain, x.Subsidi + x.Incentive + x.LainLain))
                .ToList();
            return new DaftarHargaLeasingDto(h.Id, h.Name, h.GlobalLeasingId, h.GrupTenorId, h.IsActive, items);
        }).ToList();
    }

    public Task<bool> ExistsAsync(string name, Guid globalLeasingId, Guid grupTenorId, CancellationToken ct = default)
        => context.DaftarHargaLeasingReadModels
            .AnyAsync(x => x.Name == name && x.GlobalLeasingId == globalLeasingId && x.GrupTenorId == grupTenorId, ct);

    public Task<bool> ExistsExcludingAsync(string name, Guid globalLeasingId, Guid grupTenorId, Guid excludeId, CancellationToken ct = default)
        => context.DaftarHargaLeasingReadModels
            .AnyAsync(x => x.Name == name && x.GlobalLeasingId == globalLeasingId && x.GrupTenorId == grupTenorId && x.Id != excludeId, ct);
}
