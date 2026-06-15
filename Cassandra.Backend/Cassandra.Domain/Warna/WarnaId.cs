namespace Cassandra.Domain.Warna;

public record WarnaId(Guid Value)
{
    public static WarnaId New() => new(Guid.NewGuid());
    public static WarnaId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
