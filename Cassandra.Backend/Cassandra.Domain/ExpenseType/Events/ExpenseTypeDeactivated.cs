using Cassandra.Domain.Common;

namespace Cassandra.Domain.ExpenseType.Events;

public record ExpenseTypeDeactivated(
    ExpenseTypeId ExpenseTypeId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;
