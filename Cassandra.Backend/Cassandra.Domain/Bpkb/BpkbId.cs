namespace Cassandra.Domain.Bpkb;

public record BpkbId(Guid Value)
{
    public static BpkbId New()       => new(Guid.NewGuid());
    public static BpkbId From(Guid v) => new(v);
}
