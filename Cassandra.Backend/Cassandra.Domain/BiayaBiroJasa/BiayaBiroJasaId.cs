namespace Cassandra.Domain.BiayaBiroJasa;

public record BiayaBiroJasaId(Guid Value)
{
    public static BiayaBiroJasaId New() => new(Guid.NewGuid());
    public static BiayaBiroJasaId From(Guid v) => new(v);
}
