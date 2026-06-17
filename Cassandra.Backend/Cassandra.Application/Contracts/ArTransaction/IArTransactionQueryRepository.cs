using Cassandra.Application.DTOs.ArTransaction;

namespace Cassandra.Application.Contracts.ArTransaction;

public interface IArTransactionQueryRepository
{
    Task<IReadOnlyList<ArTransactionDto>> GetAllAsync(CancellationToken ct = default);
    Task<ArTransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ArTransactionDto>> GetByReferenceIdAsync(Guid referenceId, CancellationToken ct = default);
    Task<IReadOnlyList<ArPaymentEntryDto>> GetAllPaymentEntriesAsync(string? transactionTypeFilter, CancellationToken ct = default);
}
