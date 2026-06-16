using Cassandra.Application.Contracts.MetodeKeuangan;
using Cassandra.Application.DTOs.MetodeKeuangan;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.MetodeKeuangan;

public class MetodeKeuanganQueryRepository(AppDbContext context) : IMetodeKeuanganQueryRepository
{
    public async Task<MetodeKeuanganDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.MetodeKeuanganReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new MetodeKeuanganDto(x.Id, x.Code, x.Name, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<MetodeKeuanganDto>> GetAllAsync(CancellationToken ct = default)
        => await context.MetodeKeuanganReadModels
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new MetodeKeuanganDto(x.Id, x.Code, x.Name, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.MetodeKeuanganReadModels
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.MetodeKeuanganReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.MetodeKeuanganReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);
}
