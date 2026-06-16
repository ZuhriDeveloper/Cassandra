using Cassandra.Application.DTOs.Ledger;

namespace Cassandra.Application.Contracts.Ledger;

public interface ILedgerQueryRepository
{
    Task<LedgerDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<LedgerDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);
    Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default);
}
