using Cassandra.Application.Contracts.GlobalLeasing;
using Cassandra.Application.DTOs.GlobalLeasing;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.GlobalLeasing;

public class GlobalLeasingQueryRepository(AppDbContext context) : IGlobalLeasingQueryRepository
{
    public async Task<GlobalLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.GlobalLeasingReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new GlobalLeasingDto(x.Id, x.Code, x.Name, x.Phone, x.Fax, x.Contact, x.Address, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<GlobalLeasingDto>> GetAllAsync(CancellationToken ct = default)
        => await context.GlobalLeasingReadModels
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new GlobalLeasingDto(x.Id, x.Code, x.Name, x.Phone, x.Fax, x.Contact, x.Address, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.GlobalLeasingReadModels
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.GlobalLeasingReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.GlobalLeasingReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);
}
