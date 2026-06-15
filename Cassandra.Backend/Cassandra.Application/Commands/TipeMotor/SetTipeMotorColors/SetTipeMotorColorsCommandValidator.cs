using FluentValidation;

namespace Cassandra.Application.Commands.TipeMotor.SetTipeMotorColors;

public class SetTipeMotorColorsCommandValidator : AbstractValidator<SetTipeMotorColorsCommand>
{
    public SetTipeMotorColorsCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
