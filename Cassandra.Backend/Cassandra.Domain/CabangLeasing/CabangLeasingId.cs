namespace Cassandra.Domain.CabangLeasing;

public record CabangLeasingId(Guid Value)
{
    public static CabangLeasingId New() => new(Guid.NewGuid());
    public static CabangLeasingId From(Guid v) => new(v);
}
