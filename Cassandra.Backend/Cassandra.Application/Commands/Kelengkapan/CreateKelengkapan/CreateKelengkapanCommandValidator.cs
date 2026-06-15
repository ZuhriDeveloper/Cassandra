using FluentValidation;

namespace Cassandra.Application.Commands.Kelengkapan.CreateKelengkapan;

public class CreateKelengkapanCommandValidator : AbstractValidator<CreateKelengkapanCommand>
{
    public CreateKelengkapanCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
