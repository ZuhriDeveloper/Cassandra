using Cassandra.Application.Contracts.Stock;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stock;

namespace Cassandra.Application.Commands.Stock.ChangeStockStatus;

public class ChangeStockStatusCommandHandler(IStockRepository repository)
{
    public async Task HandleAsync(ChangeStockStatusCommand command, CancellationToken ct = default)
    {
        var stock = await repository.GetByIdAsync(StockId.From(command.StockId), ct)
            ?? throw new DomainException($"Stock dengan ID '{command.StockId}' tidak ditemukan.");

        stock.ChangeStatus(command.Status, command.UpdatedBy);
        await repository.SaveAsync(stock, ct);
    }
}
