using FluentValidation;

namespace Cassandra.Application.Commands.RegistrasiPenjualan.ApproveRegistrasiPenjualan;

public class ApproveRegistrasiPenjualanCommandValidator : AbstractValidator<ApproveRegistrasiPenjualanCommand>
{
    public ApproveRegistrasiPenjualanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id registrasi penjualan tidak boleh kosong.");
        RuleFor(x => x.ApprovedDiscount).GreaterThanOrEqualTo(0).WithMessage("Diskon yang disetujui tidak boleh negatif.");
        RuleFor(x => x.ApprovedBy).NotEmpty().WithMessage("ApprovedBy tidak boleh kosong.");
    }
}
