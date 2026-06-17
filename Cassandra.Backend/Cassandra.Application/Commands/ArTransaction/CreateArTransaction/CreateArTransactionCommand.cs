namespace Cassandra.Application.Commands.ArTransaction.CreateArTransaction;

public record CreateArTransactionCommand(
    string TransactionType,
    Guid? ReferenceId,
    string ReferenceNumber,
    decimal TotalAmount,
    string CreatedBy);
