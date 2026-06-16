using FluentValidation;

namespace Cassandra.Application.Commands.MetodeKeuangan.CreateMetodeKeuangan;

public class CreateMetodeKeuanganCommandValidator : AbstractValidator<CreateMetodeKeuanganCommand>
{
    public CreateMetodeKeuanganCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
