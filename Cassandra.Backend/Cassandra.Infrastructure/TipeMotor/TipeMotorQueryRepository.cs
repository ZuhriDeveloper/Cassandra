using Cassandra.Application.Contracts.TipeMotor;
using Cassandra.Application.DTOs.TipeMotor;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.TipeMotor;

public class TipeMotorQueryRepository(AppDbContext context) : ITipeMotorQueryRepository
{
    public async Task<TipeMotorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var m = await context.TipeMotorReadModels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (m is null) return null;

        var warnaIds = await context.TipeMotorWarnaReadModels
            .AsNoTracking()
            .Where(x => x.TipeMotorId == id)
            .Select(x => x.WarnaId)
            .ToListAsync(ct);

        return ToDto(m, warnaIds);
    }

    public async Task<IReadOnlyList<TipeMotorDto>> GetAllAsync(CancellationToken ct = default)
    {
        var models = await context.TipeMotorReadModels
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .ToListAsync(ct);

        var tipeMotorIds = models.Select(x => x.Id).ToList();

        var allWarnaRows = await context.TipeMotorWarnaReadModels
            .AsNoTracking()
            .Where(x => tipeMotorIds.Contains(x.TipeMotorId))
            .ToListAsync(ct);

        var warnaByTipe = allWarnaRows
            .GroupBy(x => x.TipeMotorId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.WarnaId).ToList());

        return models
            .Select(m => ToDto(m, warnaByTipe.TryGetValue(m.Id, out var ids) ? ids : []))
            .ToList();
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => context.TipeMotorReadModels
            .IgnoreQueryFilters()
            .AnyAsync(x => x.Code == code, ct);

    private static TipeMotorDto ToDto(
        Persistence.Projections.TipeMotorReadModel m,
        IReadOnlyList<Guid> warnaIds) =>
        new(
            m.Id,
            m.Code,
            m.GrupTipeMotorId,
            m.ShortName,
            m.ProductCode,
            m.WmsCode,
            m.AhmCode,
            m.EngineNumberFormat,
            m.ChassisNumberFormat,
            m.NettPrice,
            m.OrJakarta,
            m.OrTangerang,
            m.BbnJakarta,
            m.BbnTangerang,
            m.IsActive,
            warnaIds);
}
