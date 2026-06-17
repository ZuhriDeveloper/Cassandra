using Cassandra.Application.DTOs.CashOutTransaction;

namespace Cassandra.Application.Contracts.CashOutTransaction;

public interface ICashOutTransactionQueryRepository
{
    Task<IReadOnlyList<CashOutTransactionDto>> GetAllAsync(CancellationToken ct = default);
    Task<CashOutTransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
