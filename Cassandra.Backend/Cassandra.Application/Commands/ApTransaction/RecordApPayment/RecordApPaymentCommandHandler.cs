using Cassandra.Application.Contracts.ApTransaction;
using Cassandra.Application.Contracts.Finance;
using Cassandra.Domain.ApTransaction;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.ApTransaction.RecordApPayment;

public class RecordApPaymentCommandHandler(
    IApTransactionRepository repository,
    IFinanceCounter financeCounter)
{
    public async Task<string> HandleAsync(RecordApPaymentCommand command, CancellationToken ct = default)
    {
        var apTransaction = await repository.GetByIdAsync(ApTransactionId.From(command.ApTransactionId), ct)
            ?? throw new DomainException($"Transaksi AP dengan ID '{command.ApTransactionId}' tidak ditemukan.");

        var fInvoiceId = await financeCounter.GetNextFInvoiceIdAsync(ct);
        var paymentNo = apTransaction.Payments.Count + 1;

        apTransaction.RecordPayment(
            paymentNo,
            command.Amount,
            command.PaymentDate,
            command.PaymentMethod,
            fInvoiceId,
            command.UpdatedBy,
            DateTime.UtcNow);

        await repository.SaveAsync(apTransaction, ct);
        return fInvoiceId;
    }
}
