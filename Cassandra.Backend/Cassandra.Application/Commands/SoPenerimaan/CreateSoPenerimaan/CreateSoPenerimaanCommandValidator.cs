using FluentValidation;

namespace Cassandra.Application.Commands.SoPenerimaan.CreateSoPenerimaan;

public class CreateSoPenerimaanCommandValidator : AbstractValidator<CreateSoPenerimaanCommand>
{
    public CreateSoPenerimaanCommandValidator()
    {
        RuleFor(x => x.SuratJalanId)
            .NotEmpty().WithMessage("Nomor surat jalan tidak boleh kosong.");

        RuleFor(x => x.SuratJalanDate)
            .NotEmpty().WithMessage("Tanggal surat jalan tidak boleh kosong.");

        RuleFor(x => x.SoId)
            .NotEmpty().WithMessage("SO harus dipilih.");

        RuleFor(x => x.MotorItems)
            .NotEmpty().WithMessage("Harus ada minimal satu item motor.");

        RuleForEach(x => x.MotorItems).ChildRules(item =>
        {
            item.RuleFor(i => i.TipeMotorId).NotEmpty().WithMessage("Tipe motor harus dipilih.");
            item.RuleFor(i => i.WarnaId).NotEmpty().WithMessage("Warna harus dipilih.");
            item.RuleFor(i => i.NoMesin).NotEmpty().WithMessage("Nomor mesin tidak boleh kosong.");
            item.RuleFor(i => i.NoRangka).NotEmpty().WithMessage("Nomor rangka tidak boleh kosong.");
            item.RuleFor(i => i.KiosId).NotEmpty().WithMessage("Kios harus dipilih.");
        });

        RuleForEach(x => x.KelengkapanItems).ChildRules(item =>
        {
            item.RuleFor(i => i.KelengkapanId).NotEmpty().WithMessage("Kelengkapan harus dipilih.");
            item.RuleFor(i => i.Qty).GreaterThan(0).WithMessage("Jumlah kelengkapan harus lebih dari nol.");
        });
    }
}
