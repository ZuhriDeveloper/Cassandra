namespace Cassandra.Application.Commands.ExpenseType.UpdateExpenseType;

public record UpdateExpenseTypeCommand(Guid Id, string Name, string UpdatedBy);
