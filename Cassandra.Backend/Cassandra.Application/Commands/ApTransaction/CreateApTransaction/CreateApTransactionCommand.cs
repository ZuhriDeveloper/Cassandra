namespace Cassandra.Application.Commands.ApTransaction.CreateApTransaction;

public record CreateApTransactionCommand(
    string TransactionType,
    Guid StnkId,
    string NoRangka,
    decimal TotalAmount,
    string CreatedBy);
