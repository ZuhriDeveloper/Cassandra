namespace Cassandra.Application.Commands.Stock.ChangeStockStatus;

public record ChangeStockStatusCommand(Guid StockId, string Status, string UpdatedBy);
