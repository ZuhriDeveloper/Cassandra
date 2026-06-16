using Cassandra.Domain.Ledger;

namespace Cassandra.Application.Contracts.Ledger;

public interface ILedgerRepository
{
    Task<Domain.Ledger.Ledger?> GetByIdAsync(LedgerId id, CancellationToken ct = default);
    Task SaveAsync(Domain.Ledger.Ledger ledger, CancellationToken ct = default);
}
