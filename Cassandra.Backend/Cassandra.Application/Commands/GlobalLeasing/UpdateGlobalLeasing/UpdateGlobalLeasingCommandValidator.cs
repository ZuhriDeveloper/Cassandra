using FluentValidation;

namespace Cassandra.Application.Commands.GlobalLeasing.UpdateGlobalLeasing;

public class UpdateGlobalLeasingCommandValidator : AbstractValidator<UpdateGlobalLeasingCommand>
{
    public UpdateGlobalLeasingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Fax).MaximumLength(30);
        RuleFor(x => x.Contact).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
