using FluentValidation;

namespace Cassandra.Application.Commands.GrupTenor.UpdateGrupTenor;

public class UpdateGrupTenorCommandValidator : AbstractValidator<UpdateGrupTenorCommand>
{
    public UpdateGrupTenorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
