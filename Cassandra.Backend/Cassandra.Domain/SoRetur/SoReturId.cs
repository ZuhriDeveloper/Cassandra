namespace Cassandra.Domain.SoRetur;

public record SoReturId(Guid Value)
{
    public static SoReturId New() => new(Guid.NewGuid());
    public static SoReturId From(Guid v) => new(v);
}
