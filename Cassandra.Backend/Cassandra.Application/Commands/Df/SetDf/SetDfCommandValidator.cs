using FluentValidation;

namespace Cassandra.Application.Commands.Df.SetDf;

public class SetDfCommandValidator : AbstractValidator<SetDfCommand>
{
    public SetDfCommandValidator()
    {
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Interest).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StartDate).NotEqual(default(DateOnly));
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
