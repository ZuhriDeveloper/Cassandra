using FluentValidation;

namespace Cassandra.Application.Commands.Samsat.CreateSamsat;

public class CreateSamsatCommandValidator : AbstractValidator<CreateSamsatCommand>
{
    public CreateSamsatCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
