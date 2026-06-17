using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Finance;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Finance;

public class FinanceCounterService(AppDbContext context, ICurrentDealer currentDealer) : IFinanceCounter
{
    public async Task<string> GetNextFInvoiceIdAsync(CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        var counter = await context.FinanceCounters
            .FirstOrDefaultAsync(x => x.DealerId == dealerId, ct);

        if (counter is null)
        {
            counter = new FinanceCounterReadModel
            {
                DealerId     = dealerId,
                NextSequence = 1,
            };
            context.FinanceCounters.Add(counter);
        }
        else
        {
            counter.NextSequence++;
        }

        await context.SaveChangesAsync(ct);

        return $"INV{DateTime.UtcNow:yyyyMM}{counter.NextSequence:D6}";
    }
}
