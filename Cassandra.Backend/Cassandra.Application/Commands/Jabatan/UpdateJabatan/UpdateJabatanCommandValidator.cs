using FluentValidation;

namespace Cassandra.Application.Commands.Jabatan.UpdateJabatan;

public class UpdateJabatanCommandValidator : AbstractValidator<UpdateJabatanCommand>
{
    public UpdateJabatanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
