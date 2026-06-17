using Cassandra.Domain.ArTransaction;

namespace Cassandra.Application.Contracts.ArTransaction;

public interface IArTransactionRepository
{
    Task<Domain.ArTransaction.ArTransaction?> GetByIdAsync(ArTransactionId id, CancellationToken ct = default);
    Task SaveAsync(Domain.ArTransaction.ArTransaction arTransaction, CancellationToken ct = default);
}
