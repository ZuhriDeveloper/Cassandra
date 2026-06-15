using FluentValidation;

namespace Cassandra.Application.Commands.Kelengkapan.UpdateKelengkapan;

public class UpdateKelengkapanCommandValidator : AbstractValidator<UpdateKelengkapanCommand>
{
    public UpdateKelengkapanCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UpdatedBy).NotEmpty();
    }
}
