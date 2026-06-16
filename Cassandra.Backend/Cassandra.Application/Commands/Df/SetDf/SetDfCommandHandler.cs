using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Df;

namespace Cassandra.Application.Commands.Df.SetDf;

public class SetDfCommandHandler(
    IDfRepository repository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(SetDfCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        var df = await repository.GetForDealerAsync(dealerId, ct);

        if (df is null)
            df = Domain.Df.Df.Create(command.Discount, command.Interest, command.StartDate, command.UpdatedBy, dealerId);
        else
            df.Set(command.Discount, command.Interest, command.StartDate, command.UpdatedBy);

        await repository.SaveAsync(df, ct);
        return df.Id.Value;
    }
}
