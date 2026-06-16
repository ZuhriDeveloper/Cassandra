namespace Cassandra.Domain.MetodeKeuangan;

public record MetodeKeuanganId(Guid Value)
{
    public static MetodeKeuanganId New() => new(Guid.NewGuid());
    public static MetodeKeuanganId From(Guid v) => new(v);
}
