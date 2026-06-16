namespace Cassandra.Domain.Ledger;

public record LedgerId(Guid Value)
{
    public static LedgerId New() => new(Guid.NewGuid());
    public static LedgerId From(Guid v) => new(v);
}
