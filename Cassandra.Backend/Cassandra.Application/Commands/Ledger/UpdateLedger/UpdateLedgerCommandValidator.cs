using FluentValidation;

namespace Cassandra.Application.Commands.Ledger.UpdateLedger;

public class UpdateLedgerCommandValidator : AbstractValidator<UpdateLedgerCommand>
{
    public UpdateLedgerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
