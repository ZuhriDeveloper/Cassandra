using FluentValidation;

namespace Cassandra.Application.Commands.Dealers.RegisterDealer;

public class RegisterDealerCommandValidator : AbstractValidator<RegisterDealerCommand>
{
    public RegisterDealerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.RegisteredBy).NotEmpty();
    }
}
