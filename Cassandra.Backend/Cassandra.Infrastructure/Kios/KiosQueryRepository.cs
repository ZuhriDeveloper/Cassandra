using Cassandra.Application.Contracts.Kios;
using Cassandra.Application.DTOs.Kios;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Kios;

public class KiosQueryRepository(AppDbContext context) : IKiosQueryRepository
{
    public async Task<KiosDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var m = await context.KiosReadModels.FirstOrDefaultAsync(x => x.Id == id, ct);
        return m is null ? null : ToDto(m);
    }

    public async Task<IReadOnlyList<KiosDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await context.KiosReadModels
            .OrderBy(x => x.Code)
            .ToListAsync(ct);
        return list.Select(ToDto).ToList();
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.KiosReadModels
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, ct);

    private static KiosDto ToDto(Persistence.Projections.KiosReadModel m) =>
        new(m.Id, m.Code, m.Name, m.Phone, m.Fax, m.Address, m.PicKaryawanId, m.Limit, m.IsActive);
}
