using FluentValidation;

namespace Cassandra.Application.Commands.Discount.CreateDiscount;

public class CreateDiscountCommandValidator : AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountCommandValidator()
    {
        RuleFor(x => x.DaftarHargaLeasingId).NotEmpty();
        RuleFor(x => x.Level).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
