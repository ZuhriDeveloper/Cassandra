using Cassandra.Application.Contracts.Kelengkapan;
using Cassandra.Application.DTOs.Kelengkapan;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Kelengkapan;

public class KelengkapanQueryRepository(AppDbContext context) : IKelengkapanQueryRepository
{
    public async Task<KelengkapanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var m = await context.KelengkapanReadModels.FirstOrDefaultAsync(x => x.Id == id, ct);
        return m is null ? null : ToDto(m);
    }

    public async Task<IReadOnlyList<KelengkapanDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await context.KelengkapanReadModels
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
        return list.Select(ToDto).ToList();
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.KelengkapanReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.KelengkapanReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);

    private static KelengkapanDto ToDto(Persistence.Projections.KelengkapanReadModel m) =>
        new(m.Id, m.Name, m.IsActive);
}
