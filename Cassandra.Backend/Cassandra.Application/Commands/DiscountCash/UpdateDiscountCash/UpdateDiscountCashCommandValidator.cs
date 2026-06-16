using FluentValidation;

namespace Cassandra.Application.Commands.DiscountCash.UpdateDiscountCash;

public class UpdateDiscountCashCommandValidator : AbstractValidator<UpdateDiscountCashCommand>
{
    public UpdateDiscountCashCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DirectDiscount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ChannelDiscount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
