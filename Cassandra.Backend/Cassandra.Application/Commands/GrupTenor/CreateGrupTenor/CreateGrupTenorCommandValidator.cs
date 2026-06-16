using FluentValidation;

namespace Cassandra.Application.Commands.GrupTenor.CreateGrupTenor;

public class CreateGrupTenorCommandValidator : AbstractValidator<CreateGrupTenorCommand>
{
    public CreateGrupTenorCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
