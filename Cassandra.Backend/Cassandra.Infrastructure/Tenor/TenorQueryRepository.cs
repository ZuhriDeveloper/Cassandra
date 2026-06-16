using Cassandra.Application.Contracts.Tenor;
using Cassandra.Application.DTOs.Tenor;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Tenor;

public class TenorQueryRepository(AppDbContext context) : ITenorQueryRepository
{
    public async Task<TenorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.TenorReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new TenorDto(x.Id, x.Code, x.Name, x.Months, x.GrupTenorId, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<TenorDto>> GetAllAsync(CancellationToken ct = default)
        => await context.TenorReadModels
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new TenorDto(x.Id, x.Code, x.Name, x.Months, x.GrupTenorId, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.TenorReadModels
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.TenorReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.TenorReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);
}
