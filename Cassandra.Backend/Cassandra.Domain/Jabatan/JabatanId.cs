namespace Cassandra.Domain.Jabatan;

public record JabatanId(Guid Value)
{
    public static JabatanId New() => new(Guid.NewGuid());
    public static JabatanId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
