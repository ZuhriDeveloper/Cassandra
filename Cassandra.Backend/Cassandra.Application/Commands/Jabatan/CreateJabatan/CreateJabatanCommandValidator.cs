using FluentValidation;

namespace Cassandra.Application.Commands.Jabatan.CreateJabatan;

public class CreateJabatanCommandValidator : AbstractValidator<CreateJabatanCommand>
{
    public CreateJabatanCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}
