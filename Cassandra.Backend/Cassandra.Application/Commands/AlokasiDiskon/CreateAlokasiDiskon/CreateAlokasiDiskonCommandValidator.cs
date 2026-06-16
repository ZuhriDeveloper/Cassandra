using FluentValidation;

namespace Cassandra.Application.Commands.AlokasiDiskon.CreateAlokasiDiskon;

public class CreateAlokasiDiskonCommandValidator : AbstractValidator<CreateAlokasiDiskonCommand>
{
    public CreateAlokasiDiskonCommandValidator()
    {
        RuleFor(x => x.KaryawanId).NotEmpty();
        RuleFor(x => x.DiscountLevel).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
