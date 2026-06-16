namespace Cassandra.Domain.Df;

public record DfId(Guid Value)
{
    public static DfId New() => new(Guid.NewGuid());
    public static DfId From(Guid v) => new(v);
}
