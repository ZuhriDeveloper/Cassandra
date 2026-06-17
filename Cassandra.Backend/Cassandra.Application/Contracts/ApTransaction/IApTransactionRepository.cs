using Cassandra.Domain.ApTransaction;

namespace Cassandra.Application.Contracts.ApTransaction;

public interface IApTransactionRepository
{
    Task<Domain.ApTransaction.ApTransaction?> GetByIdAsync(ApTransactionId id, CancellationToken ct = default);
    Task SaveAsync(Domain.ApTransaction.ApTransaction apTransaction, CancellationToken ct = default);
}
