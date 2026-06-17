using Cassandra.Domain.Stock;

namespace Cassandra.Application.Contracts.Stock;

public interface IStockRepository
{
    Task<Domain.Stock.Stock?> GetByIdAsync(StockId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Stock.Stock stock, CancellationToken ct = default);
}
