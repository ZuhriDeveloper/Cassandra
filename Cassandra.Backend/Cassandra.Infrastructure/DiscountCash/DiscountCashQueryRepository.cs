using Cassandra.Application.Contracts.DiscountCash;
using Cassandra.Application.DTOs.DiscountCash;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.DiscountCash;

public class DiscountCashQueryRepository(AppDbContext context) : IDiscountCashQueryRepository
{
    public async Task<DiscountCashDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.DiscountCashReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DiscountCashDto(x.Id, x.TipeMotorId, x.DirectDiscount, x.ChannelDiscount, x.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<DiscountCashDto>> GetAllAsync(CancellationToken ct = default)
        => await context.DiscountCashReadModels
            .AsNoTracking()
            .OrderBy(x => x.TipeMotorId)
            .Select(x => new DiscountCashDto(x.Id, x.TipeMotorId, x.DirectDiscount, x.ChannelDiscount, x.IsActive))
            .ToListAsync(ct);

    public Task<bool> TipeMotorIdExistsAsync(Guid tipeMotorId, CancellationToken ct = default)
        => context.DiscountCashReadModels
            .AnyAsync(x => x.TipeMotorId == tipeMotorId, ct);
}
