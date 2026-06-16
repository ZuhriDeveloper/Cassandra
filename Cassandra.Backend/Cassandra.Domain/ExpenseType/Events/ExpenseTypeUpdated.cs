using Cassandra.Domain.Common;

namespace Cassandra.Domain.ExpenseType.Events;

public record ExpenseTypeUpdated(
    ExpenseTypeId ExpenseTypeId,
    string Name,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
