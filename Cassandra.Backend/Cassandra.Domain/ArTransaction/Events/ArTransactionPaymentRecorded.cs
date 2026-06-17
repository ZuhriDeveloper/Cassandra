using Cassandra.Domain.Common;

namespace Cassandra.Domain.ArTransaction.Events;

public record ArTransactionPaymentRecorded(
    ArTransactionId Id,
    int PaymentNo,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string FInvoiceId,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
