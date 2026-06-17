using FluentValidation;

namespace Cassandra.Application.Commands.ApTransaction.RecordApPayment;

public class RecordApPaymentCommandValidator : AbstractValidator<RecordApPaymentCommand>
{
    public RecordApPaymentCommandValidator()
    {
        RuleFor(x => x.ApTransactionId).NotEmpty().WithMessage("ID transaksi AP tidak boleh kosong.");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Jumlah pembayaran harus lebih dari nol.");
        RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Metode pembayaran tidak boleh kosong.");
        RuleFor(x => x.PaymentDate).NotEmpty().WithMessage("Tanggal pembayaran tidak boleh kosong.");
        RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage("UpdatedBy tidak boleh kosong.");
    }
}
