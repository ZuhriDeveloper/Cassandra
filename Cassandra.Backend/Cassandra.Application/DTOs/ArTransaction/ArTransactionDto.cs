namespace Cassandra.Application.DTOs.ArTransaction;

public record ArPaymentEntryDto(
    int PaymentNo,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string FInvoiceId,
    string CreatedBy);

public record ArTransactionDto(
    Guid Id,
    string TransactionType,
    Guid? ReferenceId,
    string ReferenceNumber,
    decimal TotalAmount,
    decimal RemainingAmount,
    bool IsClosed,
    string CreatedBy,
    DateTime CreatedAt,
    List<ArPaymentEntryDto> Payments);
