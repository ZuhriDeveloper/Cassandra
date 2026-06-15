using FluentValidation;

namespace Cassandra.Application.Commands.Karyawan.SetKaryawanLimit;

public class SetKaryawanLimitCommandValidator : AbstractValidator<SetKaryawanLimitCommand>
{
    public SetKaryawanLimitCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SalesLimit).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SetBy).NotEmpty();
    }
}
