namespace Cassandra.Infrastructure.Persistence.Projections;

public class SamsatCityReadModel
{
    public Guid SamsatId { get; set; }
    public string City { get; set; } = default!;
}
