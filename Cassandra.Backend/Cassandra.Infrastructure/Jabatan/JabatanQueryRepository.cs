using Cassandra.Application.Contracts.Jabatan;
using Cassandra.Application.DTOs.Jabatan;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Jabatan;

public class JabatanQueryRepository(AppDbContext context) : IJabatanQueryRepository
{
    public async Task<JabatanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.JabatanReadModels
            .AsNoTracking()
            .Where(j => j.Id == id)
            .Select(j => new JabatanDto(j.Id, j.Name, j.Description, j.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<JabatanDto>> GetAllAsync(CancellationToken ct = default)
        => await context.JabatanReadModels
            .AsNoTracking()
            .OrderBy(j => j.Name)
            .Select(j => new JabatanDto(j.Id, j.Name, j.Description, j.IsActive))
            .ToListAsync(ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.JabatanReadModels.AnyAsync(j => j.Name == name, ct);
}
