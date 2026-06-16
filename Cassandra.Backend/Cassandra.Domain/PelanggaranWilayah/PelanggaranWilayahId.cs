namespace Cassandra.Domain.PelanggaranWilayah;

public record PelanggaranWilayahId(Guid Value)
{
    public static PelanggaranWilayahId New() => new(Guid.NewGuid());
    public static PelanggaranWilayahId From(Guid v) => new(v);
}
