namespace Cassandra.Domain.AlokasiDiskon;

public record AlokasiDiskonId(Guid Value)
{
    public static AlokasiDiskonId New() => new(Guid.NewGuid());
    public static AlokasiDiskonId From(Guid v) => new(v);
}
