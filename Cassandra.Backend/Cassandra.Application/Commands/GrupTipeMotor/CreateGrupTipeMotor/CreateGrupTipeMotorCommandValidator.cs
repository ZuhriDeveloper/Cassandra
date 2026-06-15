using FluentValidation;

namespace Cassandra.Application.Commands.GrupTipeMotor.CreateGrupTipeMotor;

public class CreateGrupTipeMotorCommandValidator : AbstractValidator<CreateGrupTipeMotorCommand>
{
    public CreateGrupTipeMotorCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
