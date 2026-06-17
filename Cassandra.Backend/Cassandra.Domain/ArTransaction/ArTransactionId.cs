namespace Cassandra.Domain.ArTransaction;

public record ArTransactionId(Guid Value)
{
    public static ArTransactionId New() => new(Guid.NewGuid());
    public static ArTransactionId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
