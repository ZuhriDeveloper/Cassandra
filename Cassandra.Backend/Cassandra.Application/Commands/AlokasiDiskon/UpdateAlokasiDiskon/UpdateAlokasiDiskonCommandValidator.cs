using FluentValidation;

namespace Cassandra.Application.Commands.AlokasiDiskon.UpdateAlokasiDiskon;

public class UpdateAlokasiDiskonCommandValidator : AbstractValidator<UpdateAlokasiDiskonCommand>
{
    public UpdateAlokasiDiskonCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DiscountLevel).NotEmpty().MaximumLength(20);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
