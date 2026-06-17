using Cassandra.Application.DTOs.ArTransaction;

namespace Cassandra.Application.DTOs.ApTransaction;

public record ApTransactionDto(
    Guid Id,
    string TransactionType,
    Guid StnkId,
    string NoRangka,
    decimal TotalAmount,
    decimal RemainingAmount,
    bool IsClosed,
    string CreatedBy,
    DateTime CreatedAt,
    List<ArPaymentEntryDto> Payments);
