namespace Cassandra.Domain.Biro;

public record BiroId(Guid Value)
{
    public static BiroId New() => new(Guid.NewGuid());
    public static BiroId From(Guid v) => new(v);
}
