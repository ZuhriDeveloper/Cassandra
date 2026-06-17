using FluentValidation;

namespace Cassandra.Application.Commands.Stnk.ProcessStnk;

public class ProcessStnkCommandValidator : AbstractValidator<ProcessStnkCommand>
{
    public ProcessStnkCommandValidator()
    {
        RuleFor(x => x.StnkId).NotEmpty();
        RuleFor(x => x.BiroId).NotEmpty();
        RuleFor(x => x.InvoiceNumber).NotEmpty().MaximumLength(100);
    }
}
