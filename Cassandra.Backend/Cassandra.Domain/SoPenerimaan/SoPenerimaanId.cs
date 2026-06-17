namespace Cassandra.Domain.SoPenerimaan;

public record SoPenerimaanId(Guid Value)
{
    public static SoPenerimaanId New() => new(Guid.NewGuid());
    public static SoPenerimaanId From(Guid v) => new(v);
}
