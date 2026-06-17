using Cassandra.Application.Contracts.Stock;
using Cassandra.Application.DTOs.Stock;

namespace Cassandra.Application.Queries.Stock;

public record GetStocksQuery;
public record GetStockByIdQuery(Guid Id);
public record GetStocksByKiosQuery(Guid KiosId);

public class GetStocksQueryHandler(IStockQueryRepository queryRepository)
{
    public Task<IReadOnlyList<StockDto>> HandleAsync(GetStocksQuery query, CancellationToken ct = default)
        => queryRepository.GetAllAsync(ct);

    public Task<StockDto?> HandleByIdAsync(GetStockByIdQuery query, CancellationToken ct = default)
        => queryRepository.GetByIdAsync(query.Id, ct);

    public Task<IReadOnlyList<StockDto>> HandleByKiosAsync(GetStocksByKiosQuery query, CancellationToken ct = default)
        => queryRepository.GetAvailableForKiosAsync(query.KiosId, ct);
}
