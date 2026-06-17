using Cassandra.Domain.Common;

namespace Cassandra.Domain.CashOutTransaction.Events;

public record CashOutTransactionCreated(
    CashOutTransactionId Id,
    Guid DealerId,
    string TransactionType,
    Guid? SoId,
    Guid? SoReturId,
    decimal Amount,
    decimal DfAmount,
    int TotalHariDf,
    DateTime PaymentDate,
    string PaymentMethod,
    string FInvoiceId,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
