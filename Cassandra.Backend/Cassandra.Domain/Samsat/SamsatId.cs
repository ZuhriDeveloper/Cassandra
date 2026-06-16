namespace Cassandra.Domain.Samsat;

public record SamsatId(Guid Value)
{
    public static SamsatId New() => new(Guid.NewGuid());
    public static SamsatId From(Guid v) => new(v);
}
