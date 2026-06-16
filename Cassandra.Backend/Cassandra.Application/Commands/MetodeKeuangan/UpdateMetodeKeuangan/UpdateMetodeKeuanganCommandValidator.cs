using FluentValidation;

namespace Cassandra.Application.Commands.MetodeKeuangan.UpdateMetodeKeuangan;

public class UpdateMetodeKeuanganCommandValidator : AbstractValidator<UpdateMetodeKeuanganCommand>
{
    public UpdateMetodeKeuanganCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
