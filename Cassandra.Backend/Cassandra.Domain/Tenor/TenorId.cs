namespace Cassandra.Domain.Tenor;

public record TenorId(Guid Value)
{
    public static TenorId New() => new(Guid.NewGuid());
    public static TenorId From(Guid v) => new(v);
}
