using Cassandra.Domain.Common;

namespace Cassandra.Domain.Ledger.Events;

public record LedgerDeactivated(
    LedgerId LedgerId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
