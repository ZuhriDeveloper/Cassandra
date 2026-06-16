namespace Cassandra.Domain.GrupTenor;

public record GrupTenorId(Guid Value)
{
    public static GrupTenorId New() => new(Guid.NewGuid());
    public static GrupTenorId From(Guid v) => new(v);
}
