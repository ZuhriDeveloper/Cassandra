using Cassandra.Domain.Common;

namespace Cassandra.Domain.Ledger.Events;

public record LedgerUpdated(
    LedgerId LedgerId,
    string Name,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
