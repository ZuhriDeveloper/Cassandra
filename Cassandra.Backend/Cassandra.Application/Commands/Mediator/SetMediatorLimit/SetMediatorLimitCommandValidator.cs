using FluentValidation;

namespace Cassandra.Application.Commands.Mediator.SetMediatorLimit;

public class SetMediatorLimitCommandValidator : AbstractValidator<SetMediatorLimitCommand>
{
    public SetMediatorLimitCommandValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(0).WithMessage("Limit tidak boleh negatif.");
    }
}
