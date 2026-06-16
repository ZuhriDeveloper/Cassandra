using Cassandra.Domain.Common;

namespace Cassandra.Domain.ExpenseType.Events;

public record ExpenseTypeActivated(
    ExpenseTypeId ExpenseTypeId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
