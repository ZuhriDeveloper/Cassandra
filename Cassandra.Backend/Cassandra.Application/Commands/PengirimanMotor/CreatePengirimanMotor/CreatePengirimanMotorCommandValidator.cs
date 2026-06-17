using FluentValidation;

namespace Cassandra.Application.Commands.PengirimanMotor.CreatePengirimanMotor;

public class CreatePengirimanMotorCommandValidator : AbstractValidator<CreatePengirimanMotorCommand>
{
    public CreatePengirimanMotorCommandValidator()
    {
        RuleFor(x => x.RegistrasiPenjualanId).NotEmpty().WithMessage("Registrasi penjualan harus dipilih.");
        RuleFor(x => x.NoMesin).NotEmpty().WithMessage("Nomor mesin tidak boleh kosong.");
        RuleFor(x => x.Driver1Id).NotEmpty().WithMessage("Driver 1 harus dipilih.");
        RuleFor(x => x.DeliveryDate).NotEmpty().WithMessage("Tanggal pengiriman tidak boleh kosong.");
    }
}
