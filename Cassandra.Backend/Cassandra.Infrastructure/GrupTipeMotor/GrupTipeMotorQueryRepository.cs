using Cassandra.Application.Contracts.GrupTipeMotor;
using Cassandra.Application.DTOs.GrupTipeMotor;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.GrupTipeMotor;

public class GrupTipeMotorQueryRepository(AppDbContext context) : IGrupTipeMotorQueryRepository
{
    public async Task<GrupTipeMotorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var m = await context.GrupTipeMotorReadModels.FirstOrDefaultAsync(x => x.Id == id, ct);
        return m is null ? null : ToDto(m);
    }

    public async Task<IReadOnlyList<GrupTipeMotorDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await context.GrupTipeMotorReadModels
            .OrderBy(x => x.Code)
            .ToListAsync(ct);
        return list.Select(ToDto).ToList();
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.GrupTipeMotorReadModels
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, ct);

    private static GrupTipeMotorDto ToDto(Persistence.Projections.GrupTipeMotorReadModel m) =>
        new(m.Id, m.Code, m.IsActive);
}
