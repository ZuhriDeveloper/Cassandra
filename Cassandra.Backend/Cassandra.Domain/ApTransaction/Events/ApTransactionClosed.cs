using Cassandra.Domain.Common;

namespace Cassandra.Domain.ApTransaction.Events;

public record ApTransactionClosed(
    ApTransactionId Id,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
