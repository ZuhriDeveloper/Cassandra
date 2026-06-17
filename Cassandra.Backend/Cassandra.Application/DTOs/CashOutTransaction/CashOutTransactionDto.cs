namespace Cassandra.Application.DTOs.CashOutTransaction;

public record CashOutTransactionDto(
    Guid Id,
    string TransactionType,
    Guid? SoId,
    Guid? SoReturId,
    decimal Amount,
    decimal DfAmount,
    int TotalHariDf,
    DateTime PaymentDate,
    string PaymentMethod,
    string FInvoiceId,
    bool IsClosed,
    string CreatedBy,
    DateTime CreatedAt);
