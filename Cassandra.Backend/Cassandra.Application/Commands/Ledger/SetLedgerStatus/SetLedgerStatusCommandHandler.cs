using Cassandra.Application.Contracts.Ledger;
using Cassandra.Domain.Common;
using Cassandra.Domain.Ledger;

namespace Cassandra.Application.Commands.Ledger.SetLedgerStatus;

public class SetLedgerStatusCommandHandler(ILedgerRepository repository)
{
    public async Task HandleAsync(SetLedgerStatusCommand command, CancellationToken ct = default)
    {
        var ledger = await repository.GetByIdAsync(LedgerId.From(command.Id), ct)
            ?? throw new DomainException("Ledger tidak ditemukan.");

        if (command.IsActive)
            ledger.Activate(command.UpdatedBy);
        else
            ledger.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(ledger, ct);
    }
}
