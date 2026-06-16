namespace Cassandra.Infrastructure.Persistence.Projections;

public class DiscountItemReadModel
{
    public Guid DiscountId { get; set; }
    public Guid GrupTipeMotorId { get; set; }
    public decimal Amount { get; set; }
}
