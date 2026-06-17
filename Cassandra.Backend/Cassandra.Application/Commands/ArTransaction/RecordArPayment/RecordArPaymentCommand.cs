namespace Cassandra.Application.Commands.ArTransaction.RecordArPayment;

public record RecordArPaymentCommand(
    Guid ArTransactionId,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string UpdatedBy);
