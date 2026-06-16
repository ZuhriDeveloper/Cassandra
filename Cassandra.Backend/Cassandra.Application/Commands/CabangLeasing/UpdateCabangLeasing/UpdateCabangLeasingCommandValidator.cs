using FluentValidation;

namespace Cassandra.Application.Commands.CabangLeasing.UpdateCabangLeasing;

public class UpdateCabangLeasingCommandValidator : AbstractValidator<UpdateCabangLeasingCommand>
{
    public UpdateCabangLeasingCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(30);
        RuleFor(x => x.Fax).MaximumLength(30);
        RuleFor(x => x.Contact).MaximumLength(100);
        RuleFor(x => x.GlobalLeasingId).NotEmpty();
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
