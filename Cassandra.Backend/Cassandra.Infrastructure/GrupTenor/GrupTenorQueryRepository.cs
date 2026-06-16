using Cassandra.Application.Contracts.GrupTenor;
using Cassandra.Application.DTOs.GrupTenor;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.GrupTenor;

public class GrupTenorQueryRepository(AppDbContext context) : IGrupTenorQueryRepository
{
    public async Task<GrupTenorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.GrupTenorReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new GrupTenorDto(x.Id, x.Code, x.Name, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<GrupTenorDto>> GetAllAsync(CancellationToken ct = default)
        => await context.GrupTenorReadModels
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new GrupTenorDto(x.Id, x.Code, x.Name, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.GrupTenorReadModels
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.GrupTenorReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.GrupTenorReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);
}
