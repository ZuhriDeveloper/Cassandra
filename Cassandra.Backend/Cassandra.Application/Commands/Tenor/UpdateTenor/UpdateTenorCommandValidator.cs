using FluentValidation;

namespace Cassandra.Application.Commands.Tenor.UpdateTenor;

public class UpdateTenorCommandValidator : AbstractValidator<UpdateTenorCommand>
{
    public UpdateTenorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Months).GreaterThan(0);
        RuleFor(x => x.GrupTenorId).NotEmpty();
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
