namespace Cassandra.Domain.Mutasi;

public record MutasiId(Guid Value)
{
    public static MutasiId New() => new(Guid.NewGuid());
    public static MutasiId From(Guid v) => new(v);
}
