using Cassandra.Domain.CashOutTransaction;

namespace Cassandra.Application.Contracts.CashOutTransaction;

public interface ICashOutTransactionRepository
{
    Task<Domain.CashOutTransaction.CashOutTransaction?> GetByIdAsync(CashOutTransactionId id, CancellationToken ct = default);
    Task SaveAsync(Domain.CashOutTransaction.CashOutTransaction cashOutTransaction, CancellationToken ct = default);
}
