namespace Cassandra.Domain.Dealers;

public record DealerId(Guid Value)
{
    public static DealerId New() => new(Guid.NewGuid());
    public static DealerId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
