using FluentValidation;

namespace Cassandra.Application.Commands.PelanggaranWilayah.UpdatePelanggaranWilayah;

public class UpdatePelanggaranWilayahCommandValidator : AbstractValidator<UpdatePelanggaranWilayahCommand>
{
    public UpdatePelanggaranWilayahCommandValidator()
    {
        RuleFor(x => x.ExtraFee).GreaterThanOrEqualTo(0);
    }
}
