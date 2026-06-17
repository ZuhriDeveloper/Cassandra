using Cassandra.Domain.Common;

namespace Cassandra.Domain.ApTransaction.Events;

public record ApTransactionPaymentRecorded(
    ApTransactionId Id,
    int PaymentNo,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string FInvoiceId,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
