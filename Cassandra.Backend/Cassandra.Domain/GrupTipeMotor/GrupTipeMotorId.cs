namespace Cassandra.Domain.GrupTipeMotor;

public record GrupTipeMotorId(Guid Value)
{
    public static GrupTipeMotorId New() => new(Guid.NewGuid());
    public static GrupTipeMotorId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
