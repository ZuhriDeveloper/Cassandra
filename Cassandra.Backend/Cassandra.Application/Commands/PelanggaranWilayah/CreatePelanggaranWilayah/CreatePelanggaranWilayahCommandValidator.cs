using FluentValidation;

namespace Cassandra.Application.Commands.PelanggaranWilayah.CreatePelanggaranWilayah;

public class CreatePelanggaranWilayahCommandValidator : AbstractValidator<CreatePelanggaranWilayahCommand>
{
    public CreatePelanggaranWilayahCommandValidator()
    {
        RuleFor(x => x.AreaCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ExtraFee).GreaterThanOrEqualTo(0);
    }
}
