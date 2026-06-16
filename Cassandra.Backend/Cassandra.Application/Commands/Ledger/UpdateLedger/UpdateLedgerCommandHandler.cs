using Cassandra.Application.Contracts.Ledger;
using Cassandra.Domain.Common;
using Cassandra.Domain.Ledger;

namespace Cassandra.Application.Commands.Ledger.UpdateLedger;

public class UpdateLedgerCommandHandler(
    ILedgerRepository repository,
    ILedgerQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateLedgerCommand command, CancellationToken ct = default)
    {
        var ledger = await repository.GetByIdAsync(LedgerId.From(command.Id), ct)
            ?? throw new DomainException("Ledger tidak ditemukan.");

        if (await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama ledger '{command.Name.Trim()}' sudah ada.");

        ledger.Update(command.Name, command.UpdatedBy);
        await repository.SaveAsync(ledger, ct);
    }
}
