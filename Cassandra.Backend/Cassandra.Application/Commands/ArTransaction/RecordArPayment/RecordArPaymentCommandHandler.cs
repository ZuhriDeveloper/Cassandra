using Cassandra.Application.Contracts.ArTransaction;
using Cassandra.Application.Contracts.Finance;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Domain.ArTransaction;
using Cassandra.Domain.Common;
using Cassandra.Domain.RegistrasiPenjualan;

namespace Cassandra.Application.Commands.ArTransaction.RecordArPayment;

public class RecordArPaymentCommandHandler(
    IArTransactionRepository repository,
    IFinanceCounter financeCounter,
    IRegistrasiPenjualanRepository registrasiRepo)
{
    public async Task<string> HandleAsync(RecordArPaymentCommand command, CancellationToken ct = default)
    {
        var arTransaction = await repository.GetByIdAsync(ArTransactionId.From(command.ArTransactionId), ct)
            ?? throw new DomainException($"Transaksi AR dengan ID '{command.ArTransactionId}' tidak ditemukan.");

        var fInvoiceId = await financeCounter.GetNextFInvoiceIdAsync(ct);
        var paymentNo = arTransaction.Payments.Count + 1;

        arTransaction.RecordPayment(
            paymentNo,
            command.Amount,
            command.PaymentDate,
            command.PaymentMethod,
            fInvoiceId,
            command.UpdatedBy,
            DateTime.UtcNow);

        await repository.SaveAsync(arTransaction, ct);

        // If closed and is a sale transaction, disable void on the related RegistrasiPenjualan
        if (arTransaction.IsClosed && arTransaction.ReferenceId.HasValue &&
            (arTransaction.TransactionType == Domain.ArTransaction.ArTransaction.PENJUALAN ||
             arTransaction.TransactionType == Domain.ArTransaction.ArTransaction.AMBIL_UANG ||
             arTransaction.TransactionType == Domain.ArTransaction.ArTransaction.PENJUALAN_CREDIT))
        {
            var reg = await registrasiRepo.GetByIdAsync(RegistrasiPenjualanId.From(arTransaction.ReferenceId.Value), ct);
            if (reg is not null)
            {
                reg.SetEnableToVoid(false, command.UpdatedBy);
                await registrasiRepo.SaveAsync(reg, ct);
            }
        }

        return fInvoiceId;
    }
}
