namespace Cassandra.Domain.DiscountCash;

public record DiscountCashId(Guid Value)
{
    public static DiscountCashId New() => new(Guid.NewGuid());
    public static DiscountCashId From(Guid v) => new(v);
}
