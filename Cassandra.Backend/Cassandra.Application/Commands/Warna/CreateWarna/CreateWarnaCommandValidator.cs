using FluentValidation;

namespace Cassandra.Application.Commands.Warna.CreateWarna;

public class CreateWarnaCommandValidator : AbstractValidator<CreateWarnaCommand>
{
    public CreateWarnaCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
