namespace Cassandra.Domain.Karyawan;

public record KaryawanId(Guid Value)
{
    public static KaryawanId New()        => new(Guid.NewGuid());
    public static KaryawanId From(Guid v) => new(v);
    public override string ToString()     => Value.ToString();
}
