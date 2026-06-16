using Cassandra.Application.Contracts.Ledger;
using Cassandra.Application.DTOs.Ledger;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Ledger;

public class LedgerQueryRepository(AppDbContext context) : ILedgerQueryRepository
{
    public async Task<LedgerDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.LedgerReadModels.AsNoTracking().Where(x => x.Id == id)
            .Select(x => new LedgerDto(x.Id, x.Name, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<LedgerDto>> GetAllAsync(CancellationToken ct = default)
        => await context.LedgerReadModels.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new LedgerDto(x.Id, x.Name, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.LedgerReadModels.AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.LedgerReadModels.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);
}
