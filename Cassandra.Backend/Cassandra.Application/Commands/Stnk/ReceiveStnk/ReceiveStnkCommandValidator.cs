using FluentValidation;

namespace Cassandra.Application.Commands.Stnk.ReceiveStnk;

public class ReceiveStnkCommandValidator : AbstractValidator<ReceiveStnkCommand>
{
    public ReceiveStnkCommandValidator()
    {
        RuleFor(x => x.StnkId).NotEmpty();
        RuleFor(x => x.PlateNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.BiroId).NotEmpty();
        RuleFor(x => x.StnkNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.InvoiceNumber).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StnkCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NoticeCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ProgressiveCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.BbnCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PnbpCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AdminCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OtherCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ServiceCost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PphCost).GreaterThanOrEqualTo(0);
    }
}
