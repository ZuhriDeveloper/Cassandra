using FluentValidation;

namespace Cassandra.Application.Commands.RegistrasiPenjualan.VoidRegistrasiPenjualan;

public class VoidRegistrasiPenjualanCommandValidator : AbstractValidator<VoidRegistrasiPenjualanCommand>
{
    public VoidRegistrasiPenjualanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id registrasi penjualan tidak boleh kosong.");
        RuleFor(x => x.VoidedBy).NotEmpty().WithMessage("VoidedBy tidak boleh kosong.");
    }
}
