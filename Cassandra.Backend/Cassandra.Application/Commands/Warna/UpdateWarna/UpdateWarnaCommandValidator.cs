using FluentValidation;

namespace Cassandra.Application.Commands.Warna.UpdateWarna;

public class UpdateWarnaCommandValidator : AbstractValidator<UpdateWarnaCommand>
{
    public UpdateWarnaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
