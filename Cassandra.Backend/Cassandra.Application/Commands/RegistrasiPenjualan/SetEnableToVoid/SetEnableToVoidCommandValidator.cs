using FluentValidation;

namespace Cassandra.Application.Commands.RegistrasiPenjualan.SetEnableToVoid;

public class SetEnableToVoidCommandValidator : AbstractValidator<SetEnableToVoidCommand>
{
    public SetEnableToVoidCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id registrasi penjualan tidak boleh kosong.");
        RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage("UpdatedBy tidak boleh kosong.");
    }
}
