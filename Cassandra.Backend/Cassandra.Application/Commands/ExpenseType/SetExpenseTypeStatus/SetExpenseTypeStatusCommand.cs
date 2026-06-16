namespace Cassandra.Application.Commands.ExpenseType.SetExpenseTypeStatus;

public record SetExpenseTypeStatusCommand(Guid Id, bool IsActive, string UpdatedBy);
