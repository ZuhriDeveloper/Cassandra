namespace Cassandra.Domain.Kios;

public record KiosId(Guid Value)
{
    public static KiosId New()        => new(Guid.NewGuid());
    public static KiosId From(Guid v) => new(v);
    public override string ToString() => Value.ToString();
}
