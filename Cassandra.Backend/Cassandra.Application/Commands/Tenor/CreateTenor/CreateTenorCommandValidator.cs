using FluentValidation;

namespace Cassandra.Application.Commands.Tenor.CreateTenor;

public class CreateTenorCommandValidator : AbstractValidator<CreateTenorCommand>
{
    public CreateTenorCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Months).GreaterThan(0);
        RuleFor(x => x.GrupTenorId).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
