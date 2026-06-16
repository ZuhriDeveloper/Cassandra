namespace Cassandra.Application.Commands.ExpenseType.CreateExpenseType;

public record CreateExpenseTypeCommand(string Code, string Name, string CreatedBy);
