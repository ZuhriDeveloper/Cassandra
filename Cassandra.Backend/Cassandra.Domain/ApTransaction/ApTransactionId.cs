namespace Cassandra.Domain.ApTransaction;

public record ApTransactionId(Guid Value)
{
    public static ApTransactionId New() => new(Guid.NewGuid());
    public static ApTransactionId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
