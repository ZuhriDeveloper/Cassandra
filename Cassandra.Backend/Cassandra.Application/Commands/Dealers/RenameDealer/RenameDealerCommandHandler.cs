using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers;

namespace Cassandra.Application.Commands.Dealers.RenameDealer;

public class RenameDealerCommandHandler(IDealerRepository repository)
{
    public async Task HandleAsync(RenameDealerCommand command, CancellationToken ct = default)
    {
        var dealer = await repository.GetByIdAsync(DealerId.From(command.DealerId), ct)
            ?? throw new DomainException($"Dealer '{command.DealerId}' not found.");

        dealer.Rename(command.Name, command.RenamedBy);
        await repository.SaveAsync(dealer, ct);
    }
}
