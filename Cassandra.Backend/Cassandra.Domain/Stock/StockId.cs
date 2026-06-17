namespace Cassandra.Domain.Stock;

public record StockId(Guid Value)
{
    public static StockId New() => new(Guid.NewGuid());
    public static StockId From(Guid v) => new(v);
}
