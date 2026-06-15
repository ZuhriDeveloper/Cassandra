using FluentValidation;

namespace Cassandra.Application.Commands.Kios.CreateKios;

public class CreateKiosCommandValidator : AbstractValidator<CreateKiosCommand>
{
    public CreateKiosCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Kode kios harus diisi.")
            .MaximumLength(20).WithMessage("Kode kios maksimal 20 karakter.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama kios harus diisi.")
            .MaximumLength(200).WithMessage("Nama kios maksimal 200 karakter.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Nomor telepon harus diisi.")
            .MaximumLength(20).WithMessage("Nomor telepon maksimal 20 karakter.");

        RuleFor(x => x.Fax)
            .MaximumLength(20).WithMessage("Nomor fax maksimal 20 karakter.")
            .When(x => x.Fax is not null);

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Alamat maksimal 500 karakter.");

        RuleFor(x => x.PicKaryawanId)
            .NotEmpty().WithMessage("PIC karyawan harus dipilih.");

        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(0).WithMessage("Limit tidak boleh negatif.");
    }
}
