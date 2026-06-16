using FluentValidation;

namespace Cassandra.Application.Commands.GlobalLeasing.CreateGlobalLeasing;

public class CreateGlobalLeasingCommandValidator : AbstractValidator<CreateGlobalLeasingCommand>
{
    public CreateGlobalLeasingCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Fax).MaximumLength(30);
        RuleFor(x => x.Contact).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
