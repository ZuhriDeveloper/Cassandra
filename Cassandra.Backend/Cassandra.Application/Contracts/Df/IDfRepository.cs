using Cassandra.Domain.Df;

namespace Cassandra.Application.Contracts.Df;

public interface IDfRepository
{
    Task<Domain.Df.Df?> GetForDealerAsync(Guid dealerId, CancellationToken ct = default);
    Task SaveAsync(Domain.Df.Df df, CancellationToken ct = default);
}
