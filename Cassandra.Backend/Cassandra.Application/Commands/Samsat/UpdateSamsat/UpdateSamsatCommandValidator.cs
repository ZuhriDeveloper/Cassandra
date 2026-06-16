using FluentValidation;

namespace Cassandra.Application.Commands.Samsat.UpdateSamsat;

public class UpdateSamsatCommandValidator : AbstractValidator<UpdateSamsatCommand>
{
    public UpdateSamsatCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
