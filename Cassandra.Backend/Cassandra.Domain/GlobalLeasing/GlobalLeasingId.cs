namespace Cassandra.Domain.GlobalLeasing;

public record GlobalLeasingId(Guid Value)
{
    public static GlobalLeasingId New() => new(Guid.NewGuid());
    public static GlobalLeasingId From(Guid v) => new(v);
}
