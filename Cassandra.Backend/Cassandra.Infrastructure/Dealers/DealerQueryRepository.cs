using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.DTOs.Dealers;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Dealers;

/// <summary>
/// Read side for the dealer registry. <see cref="Persistence.Projections.DealerReadModel"/>
/// is platform-level and carries no per-dealer query filter, so a SuperAdmin sees every dealer.
/// </summary>
public class DealerQueryRepository(AppDbContext context) : IDealerQueryRepository
{
    public async Task<DealerDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.DealerReadModels
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DealerDto(d.Id, d.Name, d.Code, d.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<DealerDto>> GetAllAsync(CancellationToken ct = default)
        => await context.DealerReadModels
            .AsNoTracking()
            .OrderBy(d => d.Code)
            .Select(d => new DealerDto(d.Id, d.Name, d.Code, d.IsActive))
            .ToListAsync(ct);

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.DealerReadModels.AnyAsync(d => d.Code == code, ct);
}
