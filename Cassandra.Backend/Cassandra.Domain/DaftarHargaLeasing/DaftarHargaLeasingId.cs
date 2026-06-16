namespace Cassandra.Domain.DaftarHargaLeasing;

public record DaftarHargaLeasingId(Guid Value)
{
    public static DaftarHargaLeasingId New() => new(Guid.NewGuid());
    public static DaftarHargaLeasingId From(Guid v) => new(v);
}
