namespace Cassandra.Application.Commands.CashOutTransaction.CreateCashOutTransaction;

public record CreateCashOutTransactionCommand(
    string TransactionType,
    Guid? SoId,
    Guid? SoReturId,
    decimal Amount,
    decimal DfAmount,
    int TotalHariDf,
    DateTime PaymentDate,
    string PaymentMethod,
    string CreatedBy);
