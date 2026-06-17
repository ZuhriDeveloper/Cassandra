using Cassandra.Application.Contracts.SoRetur;
using Cassandra.Application.DTOs.SoRetur;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.SoRetur;

public class SoReturQueryRepository(AppDbContext context) : ISoReturQueryRepository
{
    public async Task<IReadOnlyList<SoReturDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.SoReturReadModels
            .AsNoTracking()
            .OrderByDescending(x => x.ReturDate)
            .Select(x => new SoReturDto(
                x.Id, x.ReturNumber, x.SoId, x.ReturDate, x.Reason,
                x.Total, x.PPn, x.TotalAmount, null))
            .ToListAsync(ct);
    }

    public async Task<SoReturDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.SoReturReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : new SoReturDto(
            row.Id, row.ReturNumber, row.SoId, row.ReturDate, row.Reason,
            row.Total, row.PPn, row.TotalAmount, null);
    }

    public async Task<SoReturDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.SoReturReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        if (row is null) return null;

        var items = await context.SoReturItemReadModels
            .AsNoTracking()
            .Where(x => x.SoReturId == id)
            .Select(x => new SoReturItemDto(x.TipeMotorId, x.WarnaId, x.Qty, x.NettPrice))
            .ToListAsync(ct);

        return new SoReturDto(
            row.Id, row.ReturNumber, row.SoId, row.ReturDate, row.Reason,
            row.Total, row.PPn, row.TotalAmount, items);
    }

    public Task<bool> ReturNumberExistsAsync(string returNumber, CancellationToken ct = default)
        => context.SoReturReadModels.AnyAsync(x => x.ReturNumber == returNumber, ct);
}
