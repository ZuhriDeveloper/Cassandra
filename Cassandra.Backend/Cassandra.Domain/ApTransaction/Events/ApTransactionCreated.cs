using Cassandra.Domain.Common;

namespace Cassandra.Domain.ApTransaction.Events;

public record ApTransactionCreated(
    ApTransactionId Id,
    Guid DealerId,
    string TransactionType,
    Guid StnkId,
    string NoRangka,
    decimal TotalAmount,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
