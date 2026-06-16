using Cassandra.Application.Contracts.Biro;
using Cassandra.Application.DTOs.Biro;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Biro;

public class BiroQueryRepository(AppDbContext context) : IBiroQueryRepository
{
    public async Task<BiroDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.BiroReadModels.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new BiroDto(x.Id, x.Code, x.Name, x.Phone, x.Fax, x.Pic, x.Address, x.PphRate, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<BiroDto>> GetAllAsync(CancellationToken ct = default)
        => await context.BiroReadModels.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new BiroDto(x.Id, x.Code, x.Name, x.Phone, x.Fax, x.Pic, x.Address, x.PphRate, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.BiroReadModels.IgnoreQueryFilters().AnyAsync(x => x.Code == code, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.BiroReadModels.AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.BiroReadModels.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);
}
