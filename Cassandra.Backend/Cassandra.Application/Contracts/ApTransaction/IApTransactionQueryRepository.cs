using Cassandra.Application.DTOs.ApTransaction;
using Cassandra.Application.DTOs.ArTransaction;

namespace Cassandra.Application.Contracts.ApTransaction;

public interface IApTransactionQueryRepository
{
    Task<IReadOnlyList<ApTransactionDto>> GetAllAsync(CancellationToken ct = default);
    Task<ApTransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ApTransactionDto>> GetByStnkIdAsync(Guid stnkId, CancellationToken ct = default);
    Task<IReadOnlyList<ArPaymentEntryDto>> GetAllPaymentEntriesAsync(string? transactionTypeFilter, CancellationToken ct = default);
}
