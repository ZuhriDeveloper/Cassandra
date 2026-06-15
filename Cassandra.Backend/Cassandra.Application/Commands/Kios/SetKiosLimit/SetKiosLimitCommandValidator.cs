using FluentValidation;

namespace Cassandra.Application.Commands.Kios.SetKiosLimit;

public class SetKiosLimitCommandValidator : AbstractValidator<SetKiosLimitCommand>
{
    public SetKiosLimitCommandValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(0).WithMessage("Limit tidak boleh negatif.");
    }
}
