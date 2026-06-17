using FluentValidation;

namespace Cassandra.Application.Commands.Mutasi.CreateMutasi;

public class CreateMutasiCommandValidator : AbstractValidator<CreateMutasiCommand>
{
    public CreateMutasiCommandValidator()
    {
        RuleFor(x => x.MutasiNumber)
            .NotEmpty().WithMessage("Nomor mutasi tidak boleh kosong.");

        RuleFor(x => x.MutasiDate)
            .NotEmpty().WithMessage("Tanggal mutasi tidak boleh kosong.");

        RuleFor(x => x.SourceKiosId)
            .NotEmpty().WithMessage("Kios asal harus dipilih.");

        RuleFor(x => x.DestinationKiosId)
            .NotEmpty().WithMessage("Kios tujuan harus dipilih.");

        RuleFor(x => x.EngineNumbers)
            .NotEmpty().WithMessage("Mutasi harus memiliki minimal satu nomor mesin.");

        RuleForEach(x => x.EngineNumbers)
            .NotEmpty().WithMessage("Nomor mesin tidak boleh kosong.");

        RuleForEach(x => x.KelengkapanItems).ChildRules(item =>
        {
            item.RuleFor(i => i.KelengkapanName).NotEmpty().WithMessage("Nama kelengkapan tidak boleh kosong.");
            item.RuleFor(i => i.Qty).GreaterThan(0).WithMessage("Jumlah kelengkapan harus lebih dari nol.");
        });
    }
}
