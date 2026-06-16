namespace Cassandra.Domain.Discount;

public record DiscountId(Guid Value)
{
    public static DiscountId New() => new(Guid.NewGuid());
    public static DiscountId From(Guid v) => new(v);
}
