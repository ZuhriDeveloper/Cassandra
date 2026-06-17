using Cassandra.Application.Contracts.Stnk;
using Cassandra.Application.DTOs.Stnk;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Stnk;

public class StnkQueryRepository(AppDbContext context) : IStnkQueryRepository
{
    public async Task<IReadOnlyList<StnkDto>> GetAllAsync(CancellationToken ct = default)
        => await context.StnkReadModels
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

    public async Task<StnkDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.StnkReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : ToDto(row);
    }

    public Task<bool> ExistsByRegistrasiPenjualanIdAsync(Guid registrasiPenjualanId, CancellationToken ct = default)
        => context.StnkReadModels
            .AnyAsync(x => x.RegistrasiPenjualanId == registrasiPenjualanId, ct);

    private static StnkDto ToDto(Persistence.Projections.StnkReadModel x)
        => new(
            x.Id,
            x.RegistrasiPenjualanId,
            x.Status,
            x.FakturDate,
            x.FakturName,
            x.FakturAddress,
            x.ProcessDate,
            x.BiroId,
            x.InvoiceNumber,
            x.PlateNumber,
            x.StnkNumber,
            x.StnkCost,
            x.ProgressiveCost,
            x.NoticeCost,
            x.ReceiveDate,
            x.HandoverDate,
            x.StnkReceiver,
            x.Region,
            x.BbnCost,
            x.PnbpCost,
            x.AdminCost,
            x.OtherCost,
            x.ServiceCost,
            x.PphCost,
            x.IsInvoiceValid);
}
