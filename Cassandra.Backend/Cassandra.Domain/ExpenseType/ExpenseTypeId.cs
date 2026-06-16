namespace Cassandra.Domain.ExpenseType;

public record ExpenseTypeId(Guid Value)
{
    public static ExpenseTypeId New() => new(Guid.NewGuid());
    public static ExpenseTypeId From(Guid v) => new(v);
}
