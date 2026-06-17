using Cassandra.Domain.Common;

namespace Cassandra.Domain.ArTransaction.Events;

public record ArTransactionClosed(
    ArTransactionId Id,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
