using Cassandra.Application.Contracts.Warna;
using Cassandra.Application.DTOs.Warna;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Warna;

public class WarnaQueryRepository(AppDbContext context) : IWarnaQueryRepository
{
    public async Task<WarnaDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var m = await context.WarnaReadModels.FirstOrDefaultAsync(x => x.Id == id, ct);
        return m is null ? null : ToDto(m);
    }

    public async Task<IReadOnlyList<WarnaDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await context.WarnaReadModels
            .OrderBy(x => x.Code)
            .ToListAsync(ct);
        return list.Select(ToDto).ToList();
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.WarnaReadModels
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, ct);

    private static WarnaDto ToDto(Persistence.Projections.WarnaReadModel m) =>
        new(m.Id, m.Code, m.Name, m.IsActive);
}
