using Cassandra.Domain.Common;

namespace Cassandra.Domain.ArTransaction.Events;

public record ArTransactionCreated(
    ArTransactionId Id,
    Guid DealerId,
    string TransactionType,
    Guid? ReferenceId,
    string ReferenceNumber,
    decimal TotalAmount,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
