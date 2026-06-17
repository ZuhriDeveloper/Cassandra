using Cassandra.Domain.So;
using FluentValidation;

namespace Cassandra.Application.Commands.So.CreateSo;

public class CreateSoCommandValidator : AbstractValidator<CreateSoCommand>
{
    public CreateSoCommandValidator()
    {
        RuleFor(x => x.SoNumber)
            .NotEmpty().WithMessage("Nomor SO tidak boleh kosong.");

        RuleFor(x => x.SoDate)
            .NotEmpty().WithMessage("Tanggal SO tidak boleh kosong.");

        RuleFor(x => x.PaymentType)
            .Must(p => p == SoPaymentType.CASH || p == SoPaymentType.DF)
            .WithMessage("Tipe pembayaran harus CASH atau DF.");

        RuleFor(x => x.MetodeKeuanganId)
            .NotEmpty().WithMessage("Metode keuangan harus dipilih.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("SO harus memiliki minimal satu item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.TipeMotorId).NotEmpty().WithMessage("Tipe motor harus dipilih.");
            item.RuleFor(i => i.WarnaId).NotEmpty().WithMessage("Warna harus dipilih.");
            item.RuleFor(i => i.Qty).GreaterThan(0).WithMessage("Jumlah harus lebih dari nol.");
            item.RuleFor(i => i.NettPrice).GreaterThanOrEqualTo(0).WithMessage("Harga tidak boleh negatif.");
        });
    }
}
