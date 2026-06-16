using FluentValidation;

namespace Cassandra.Application.Commands.Discount.UpdateDiscount;

public class UpdateDiscountCommandValidator : AbstractValidator<UpdateDiscountCommand>
{
    public UpdateDiscountCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DaftarHargaLeasingId).NotEmpty();
        RuleFor(x => x.Level).NotEmpty().MaximumLength(20);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
