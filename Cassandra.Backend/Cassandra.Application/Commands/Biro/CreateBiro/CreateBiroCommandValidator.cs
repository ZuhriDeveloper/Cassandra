using FluentValidation;

namespace Cassandra.Application.Commands.Biro.CreateBiro;

public class CreateBiroCommandValidator : AbstractValidator<CreateBiroCommand>
{
    public CreateBiroCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(100);
        RuleFor(x => x.Fax).MaximumLength(100);
        RuleFor(x => x.Pic).MaximumLength(100);
        RuleFor(x => x.Address).MaximumLength(500);
        RuleFor(x => x.PphRate).GreaterThanOrEqualTo(0);
    }
}
