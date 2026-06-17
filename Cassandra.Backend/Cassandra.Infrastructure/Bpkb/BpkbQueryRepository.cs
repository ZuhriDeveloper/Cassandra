using Cassandra.Application.Contracts.Bpkb;
using Cassandra.Application.DTOs.Bpkb;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Bpkb;

public class BpkbQueryRepository(AppDbContext context) : IBpkbQueryRepository
{
    public async Task<IReadOnlyList<BpkbDto>> GetAllAsync(CancellationToken ct = default)
        => await context.BpkbReadModels
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

    public async Task<BpkbDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.BpkbReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : ToDto(row);
    }

    private static BpkbDto ToDto(Persistence.Projections.BpkbReadModel x)
        => new(
            x.Id,
            x.RegistrasiPenjualanId,
            x.StnkId,
            x.Status,
            x.RequestDate,
            x.BpkbNumber,
            x.BookNumber,
            x.ReceiveDate,
            x.HandoverDate,
            x.Receiver);
}
