using Cassandra.Application.Contracts.Discount;
using Cassandra.Application.DTOs.Discount;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Discount;

public class DiscountQueryRepository(AppDbContext context) : IDiscountQueryRepository
{
    public async Task<DiscountDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var header = await context.DiscountReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        if (header is null) return null;

        var items = await context.DiscountItemReadModels
            .AsNoTracking()
            .Where(x => x.DiscountId == id)
            .Select(x => new DiscountItemDto(x.GrupTipeMotorId, x.Amount))
            .ToListAsync(ct);

        return new DiscountDto(header.Id, header.DaftarHargaLeasingId, header.Level, header.IsActive, items);
    }

    public async Task<IReadOnlyList<DiscountDto>> GetAllAsync(CancellationToken ct = default)
    {
        var headers = await context.DiscountReadModels
            .AsNoTracking()
            .OrderBy(x => x.Level)
            .ToListAsync(ct);

        if (headers.Count == 0) return [];

        var ids = headers.Select(h => h.Id).ToList();
        var allItems = await context.DiscountItemReadModels
            .AsNoTracking()
            .Where(x => ids.Contains(x.DiscountId))
            .ToListAsync(ct);

        return headers.Select(h =>
        {
            var items = allItems
                .Where(x => x.DiscountId == h.Id)
                .Select(x => new DiscountItemDto(x.GrupTipeMotorId, x.Amount))
                .ToList();
            return new DiscountDto(h.Id, h.DaftarHargaLeasingId, h.Level, h.IsActive, items);
        }).ToList();
    }

    public Task<bool> ExistsAsync(Guid daftarHargaLeasingId, string level, CancellationToken ct = default)
        => context.DiscountReadModels
            .AnyAsync(x => x.DaftarHargaLeasingId == daftarHargaLeasingId && x.Level == level, ct);

    public Task<bool> ExistsExcludingAsync(Guid daftarHargaLeasingId, string level, Guid excludeId, CancellationToken ct = default)
        => context.DiscountReadModels
            .AnyAsync(x => x.DaftarHargaLeasingId == daftarHargaLeasingId && x.Level == level && x.Id != excludeId, ct);
}
