using Cassandra.Application.DTOs.Stock;

namespace Cassandra.Application.Contracts.Stock;

public interface IStockQueryRepository
{
    Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken ct = default);
    Task<StockDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<StockDto?> GetByNoMesinAsync(string noMesin, CancellationToken ct = default);
    Task<bool> NoMesinExistsAsync(string noMesin, CancellationToken ct = default);
    Task<IReadOnlyList<StockDto>> GetAvailableForKiosAsync(Guid kiosId, CancellationToken ct = default);
    Task<IReadOnlyList<StockDto>> GetAllByTipeMotorAsync(Guid tipeMotorId, CancellationToken ct = default);
}
