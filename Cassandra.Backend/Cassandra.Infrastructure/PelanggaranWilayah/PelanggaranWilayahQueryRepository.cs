using Cassandra.Application.Contracts.PelanggaranWilayah;
using Cassandra.Application.DTOs.PelanggaranWilayah;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.PelanggaranWilayah;

public class PelanggaranWilayahQueryRepository(AppDbContext context) : IPelanggaranWilayahQueryRepository
{
    public async Task<PelanggaranWilayahDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.PelanggaranWilayahReadModels.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new PelanggaranWilayahDto(x.Id, x.AreaCode, x.ExtraFee, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<PelanggaranWilayahDto>> GetAllAsync(CancellationToken ct = default)
        => await context.PelanggaranWilayahReadModels.AsNoTracking().OrderBy(x => x.AreaCode)
            .Select(x => new PelanggaranWilayahDto(x.Id, x.AreaCode, x.ExtraFee, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> AreaCodeExistsAsync(string areaCode, CancellationToken ct = default)
        => context.PelanggaranWilayahReadModels.IgnoreQueryFilters().AnyAsync(x => x.AreaCode == areaCode, ct);
}
