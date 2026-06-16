using Cassandra.Application.Contracts.CabangLeasing;
using Cassandra.Application.DTOs.CabangLeasing;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.CabangLeasing;

public class CabangLeasingQueryRepository(AppDbContext context) : ICabangLeasingQueryRepository
{
    public async Task<CabangLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.CabangLeasingReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CabangLeasingDto(x.Id, x.Code, x.Name, x.Phone, x.Fax, x.Contact, x.GlobalLeasingId, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<CabangLeasingDto>> GetAllAsync(CancellationToken ct = default)
        => await context.CabangLeasingReadModels
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CabangLeasingDto(x.Id, x.Code, x.Name, x.Phone, x.Fax, x.Contact, x.GlobalLeasingId, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.CabangLeasingReadModels
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.CabangLeasingReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.CabangLeasingReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);
}
