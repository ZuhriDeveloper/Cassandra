namespace Cassandra.Application.DTOs.Finance;

public record FInvoiceDto(
    string FInvoiceId,
    string TransactionType,
    string ReferenceNumber,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string CreatedBy);
