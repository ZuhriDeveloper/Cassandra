using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Ledger;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Ledger.CreateLedger;

public class CreateLedgerCommandHandler(
    ILedgerRepository repository,
    ILedgerQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateLedgerCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama ledger '{command.Name.Trim()}' sudah ada.");

        var ledger = Domain.Ledger.Ledger.Create(command.Name, command.CreatedBy, dealerId);
        await repository.SaveAsync(ledger, ct);
        return ledger.Id.Value;
    }
}
