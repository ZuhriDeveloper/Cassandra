using FluentValidation;

namespace Cassandra.Application.Commands.CabangLeasing.CreateCabangLeasing;

public class CreateCabangLeasingCommandValidator : AbstractValidator<CreateCabangLeasingCommand>
{
    public CreateCabangLeasingCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Fax).MaximumLength(30);
        RuleFor(x => x.Contact).MaximumLength(100);
        RuleFor(x => x.GlobalLeasingId).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
