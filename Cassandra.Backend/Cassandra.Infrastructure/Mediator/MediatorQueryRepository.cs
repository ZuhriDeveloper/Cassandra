using Cassandra.Application.Contracts.Mediator;
using Cassandra.Application.DTOs.Mediator;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Mediator;

public class MediatorQueryRepository(AppDbContext context) : IMediatorQueryRepository
{
    public async Task<MediatorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var m = await context.MediatorReadModels.FirstOrDefaultAsync(x => x.Id == id, ct);
        return m is null ? null : ToDto(m);
    }

    public async Task<IReadOnlyList<MediatorDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await context.MediatorReadModels
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
        return list.Select(ToDto).ToList();
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.MediatorReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.MediatorReadModels
            .AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);

    private static MediatorDto ToDto(Persistence.Projections.MediatorReadModel m) =>
        new(m.Id, m.Name, m.KaryawanId, m.Address, m.Limit, m.IsActive);
}
