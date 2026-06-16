using Cassandra.Application.Contracts.AlokasiDiskon;
using Cassandra.Application.DTOs.AlokasiDiskon;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.AlokasiDiskon;

public class AlokasiDiskonQueryRepository(AppDbContext context) : IAlokasiDiskonQueryRepository
{
    public async Task<AlokasiDiskonDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.AlokasiDiskonReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AlokasiDiskonDto(x.Id, x.KaryawanId, x.DiscountLevel, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<AlokasiDiskonDto>> GetAllAsync(CancellationToken ct = default)
        => await context.AlokasiDiskonReadModels
            .AsNoTracking()
            .OrderBy(x => x.KaryawanId)
            .Select(x => new AlokasiDiskonDto(x.Id, x.KaryawanId, x.DiscountLevel, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> KaryawanIdExistsAsync(Guid karyawanId, CancellationToken ct = default)
        => context.AlokasiDiskonReadModels
            .AnyAsync(x => x.KaryawanId == karyawanId, ct);
}
