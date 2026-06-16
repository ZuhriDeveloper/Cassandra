using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Df;
using Cassandra.Application.DTOs.Df;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Df;

public class DfQueryRepository(AppDbContext context, ICurrentDealer currentDealer) : IDfQueryRepository
{
    public async Task<DfDto?> GetForDealerAsync(CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        return await context.DfReadModels
            .AsNoTracking()
            .Where(x => x.DealerId == dealerId)
            .Select(x => new DfDto(x.Id, x.Discount, x.Interest, x.StartDate))
            .FirstOrDefaultAsync(ct);
    }
}
