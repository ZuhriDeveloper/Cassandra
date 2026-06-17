namespace Cassandra.Domain.RegistrasiPenjualan;

public record RegistrasiPenjualanId(Guid Value)
{
    public static RegistrasiPenjualanId New()  => new(Guid.NewGuid());
    public static RegistrasiPenjualanId From(Guid v) => new(v);
}
