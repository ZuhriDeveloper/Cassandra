using Cassandra.Application.Contracts.PengirimanMotor;
using Cassandra.Application.DTOs.PengirimanMotor;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.PengirimanMotor;

public class PengirimanMotorQueryRepository(AppDbContext context) : IPengirimanMotorQueryRepository
{
    public async Task<IReadOnlyList<PengirimanMotorDto>> GetAllAsync(CancellationToken ct = default)
        => await context.PengirimanMotorReadModels
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PengirimanMotorDto(
                x.Id,
                x.RegistrasiPenjualanId,
                x.NoMesin,
                x.Driver1Id,
                x.Driver2Id,
                x.DeliveryDate,
                x.Zona,
                x.CreatedBy,
                x.CreatedAt))
            .ToListAsync(ct);

    public async Task<PengirimanMotorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var row = await context.PengirimanMotorReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        return row is null ? null : new PengirimanMotorDto(
            row.Id,
            row.RegistrasiPenjualanId,
            row.NoMesin,
            row.Driver1Id,
            row.Driver2Id,
            row.DeliveryDate,
            row.Zona,
            row.CreatedBy,
            row.CreatedAt);
    }
}
