namespace Cassandra.Domain.So;

public record SoId(Guid Value)
{
    public static SoId New() => new(Guid.NewGuid());
    public static SoId From(Guid v) => new(v);
}
