namespace Cassandra.Domain.PengirimanMotor;

public record PengirimanMotorId(Guid Value)
{
    public static PengirimanMotorId New()      => new(Guid.NewGuid());
    public static PengirimanMotorId From(Guid v) => new(v);
}
