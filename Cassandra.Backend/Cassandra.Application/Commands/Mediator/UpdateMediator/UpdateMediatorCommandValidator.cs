using FluentValidation;

namespace Cassandra.Application.Commands.Mediator.UpdateMediator;

public class UpdateMediatorCommandValidator : AbstractValidator<UpdateMediatorCommand>
{
    public UpdateMediatorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama mediator harus diisi.")
            .MaximumLength(200).WithMessage("Nama mediator maksimal 200 karakter.");

        RuleFor(x => x.KaryawanId)
            .NotEmpty().WithMessage("Karyawan harus dipilih.");

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Alamat maksimal 500 karakter.");
    }
}
