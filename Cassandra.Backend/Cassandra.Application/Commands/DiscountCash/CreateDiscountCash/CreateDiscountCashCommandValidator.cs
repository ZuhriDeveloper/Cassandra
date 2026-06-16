using FluentValidation;

namespace Cassandra.Application.Commands.DiscountCash.CreateDiscountCash;

public class CreateDiscountCashCommandValidator : AbstractValidator<CreateDiscountCashCommand>
{
    public CreateDiscountCashCommandValidator()
    {
        RuleFor(x => x.TipeMotorId).NotEmpty();
        RuleFor(x => x.DirectDiscount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ChannelDiscount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
