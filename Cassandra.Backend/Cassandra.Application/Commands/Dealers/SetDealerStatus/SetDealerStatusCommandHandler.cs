using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers;

namespace Cassandra.Application.Commands.Dealers.SetDealerStatus;

public class SetDealerStatusCommandHandler(IDealerRepository repository)
{
    public async Task HandleAsync(SetDealerStatusCommand command, CancellationToken ct = default)
    {
        var dealer = await repository.GetByIdAsync(DealerId.From(command.DealerId), ct)
            ?? throw new DomainException($"Dealer '{command.DealerId}' not found.");

        if (command.IsActive)
            dealer.Activate(command.UpdatedBy);
        else
            dealer.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(dealer, ct);
    }
}
