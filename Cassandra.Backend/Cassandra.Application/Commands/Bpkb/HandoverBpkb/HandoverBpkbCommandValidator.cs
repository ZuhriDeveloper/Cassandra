using FluentValidation;

namespace Cassandra.Application.Commands.Bpkb.HandoverBpkb;

public class HandoverBpkbCommandValidator : AbstractValidator<HandoverBpkbCommand>
{
    public HandoverBpkbCommandValidator()
    {
        RuleFor(x => x.BpkbId).NotEmpty();
        RuleFor(x => x.Receiver).NotEmpty().MaximumLength(200);
    }
}
