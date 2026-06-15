namespace Cassandra.Domain.Kelengkapan;

public record KelengkapanId(Guid Value)
{
    public static KelengkapanId New() => new(Guid.NewGuid());
    public static KelengkapanId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
