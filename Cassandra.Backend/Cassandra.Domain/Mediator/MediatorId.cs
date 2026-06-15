namespace Cassandra.Domain.Mediator;

public record MediatorId(Guid Value)
{
    public static MediatorId New()        => new(Guid.NewGuid());
    public static MediatorId From(Guid v) => new(v);
    public override string ToString()     => Value.ToString();
}
