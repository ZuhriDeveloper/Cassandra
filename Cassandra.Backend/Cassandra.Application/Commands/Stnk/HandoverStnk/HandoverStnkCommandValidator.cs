using FluentValidation;

namespace Cassandra.Application.Commands.Stnk.HandoverStnk;

public class HandoverStnkCommandValidator : AbstractValidator<HandoverStnkCommand>
{
    public HandoverStnkCommandValidator()
    {
        RuleFor(x => x.StnkId).NotEmpty();
        RuleFor(x => x.StnkReceiver).NotEmpty().MaximumLength(200);
    }
}
