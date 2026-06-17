using FluentValidation;

namespace Cassandra.Application.Commands.Bpkb.ReceiveBpkb;

public class ReceiveBpkbCommandValidator : AbstractValidator<ReceiveBpkbCommand>
{
    public ReceiveBpkbCommandValidator()
    {
        RuleFor(x => x.BpkbId).NotEmpty();
        RuleFor(x => x.BpkbNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BookNumber).NotEmpty().MaximumLength(100);
    }
}
