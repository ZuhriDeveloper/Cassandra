namespace Cassandra.Application.Commands.ApTransaction.RecordApPayment;

public record RecordApPaymentCommand(
    Guid ApTransactionId,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string UpdatedBy);
