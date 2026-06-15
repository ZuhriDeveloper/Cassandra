namespace Cassandra.Domain.TipeMotor;

public record TipeMotorId(Guid Value)
{
    public static TipeMotorId New() => new(Guid.NewGuid());
    public static TipeMotorId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
