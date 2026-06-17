using FluentValidation;

namespace Cassandra.Application.Commands.ArTransaction.RecordArPayment;

public class RecordArPaymentCommandValidator : AbstractValidator<RecordArPaymentCommand>
{
    public RecordArPaymentCommandValidator()
    {
        RuleFor(x => x.ArTransactionId).NotEmpty().WithMessage("ID transaksi AR tidak boleh kosong.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Jumlah pembayaran harus lebih dari nol.");
        RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Metode pembayaran tidak boleh kosong.");
        RuleFor(x => x.PaymentDate).NotEmpty().WithMessage("Tanggal pembayaran tidak boleh kosong.");
        RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage("UpdatedBy tidak boleh kosong.");
    }
}
