using Cassandra.Domain.Dealers;

namespace Cassandra.Application.Contracts.Dealers;

public interface IDealerRepository
{
    Task<Dealer?> GetByIdAsync(DealerId id, CancellationToken ct = default);
    Task SaveAsync(Dealer dealer, CancellationToken ct = default);
}
