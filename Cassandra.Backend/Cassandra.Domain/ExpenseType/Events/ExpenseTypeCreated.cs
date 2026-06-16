using Cassandra.Domain.Common;

namespace Cassandra.Domain.ExpenseType.Events;

public record ExpenseTypeCreated(
    ExpenseTypeId ExpenseTypeId,
    Guid DealerId,
    string Code,
    string Name,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
