namespace Cassandra.Domain.Stnk;

public record StnkId(Guid Value)
{
    public static StnkId New()      => new(Guid.NewGuid());
    public static StnkId From(Guid v) => new(v);
}
