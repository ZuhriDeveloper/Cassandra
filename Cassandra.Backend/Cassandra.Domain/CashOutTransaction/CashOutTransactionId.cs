namespace Cassandra.Domain.CashOutTransaction;

public record CashOutTransactionId(Guid Value)
{
    public static CashOutTransactionId New() => new(Guid.NewGuid());
    public static CashOutTransactionId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
