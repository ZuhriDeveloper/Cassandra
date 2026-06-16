using Cassandra.Domain.Common;

namespace Cassandra.Domain.Ledger.Events;

public record LedgerCreated(
    LedgerId LedgerId,
    Guid DealerId,
    string Name,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
