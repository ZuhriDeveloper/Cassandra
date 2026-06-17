using FluentValidation;

namespace Cassandra.Application.Commands.SoRetur.CreateSoRetur;

public class CreateSoReturCommandValidator : AbstractValidator<CreateSoReturCommand>
{
    public CreateSoReturCommandValidator()
    {
        RuleFor(x => x.ReturNumber)
            .NotEmpty().WithMessage("Nomor retur tidak boleh kosong.");

        RuleFor(x => x.SoId)
            .NotEmpty().WithMessage("SO harus dipilih.");

        RuleFor(x => x.ReturDate)
            .NotEmpty().WithMessage("Tanggal retur tidak boleh kosong.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Alasan retur tidak boleh kosong.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Retur harus memiliki minimal satu item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.TipeMotorId).NotEmpty().WithMessage("Tipe motor harus dipilih.");
            item.RuleFor(i => i.WarnaId).NotEmpty().WithMessage("Warna harus dipilih.");
            item.RuleFor(i => i.Qty).GreaterThan(0).WithMessage("Jumlah harus lebih dari nol.");
            item.RuleFor(i => i.NettPrice).GreaterThanOrEqualTo(0).WithMessage("Harga tidak boleh negatif.");
        });
    }
}
