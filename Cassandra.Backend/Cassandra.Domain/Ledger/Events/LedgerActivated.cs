using Cassandra.Domain.Common;

namespace Cassandra.Domain.Ledger.Events;

public record LedgerActivated(
    LedgerId LedgerId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
