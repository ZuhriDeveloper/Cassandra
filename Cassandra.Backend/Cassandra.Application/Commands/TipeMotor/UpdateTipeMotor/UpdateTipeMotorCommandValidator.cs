using FluentValidation;

namespace Cassandra.Application.Commands.TipeMotor.UpdateTipeMotor;

public class UpdateTipeMotorCommandValidator : AbstractValidator<UpdateTipeMotorCommand>
{
    public UpdateTipeMotorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.GrupTipeMotorId).NotEmpty();
        RuleFor(x => x.ShortName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ProductCode).MaximumLength(100);
        RuleFor(x => x.WmsCode).MaximumLength(100);
        RuleFor(x => x.AhmCode).MaximumLength(100);
        RuleFor(x => x.EngineNumberFormat).MaximumLength(100);
        RuleFor(x => x.ChassisNumberFormat).MaximumLength(100);
        RuleFor(x => x.NettPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OrJakarta).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OrTangerang).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BbnJakarta).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BbnTangerang).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
