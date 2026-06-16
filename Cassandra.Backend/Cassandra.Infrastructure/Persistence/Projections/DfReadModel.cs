namespace Cassandra.Infrastructure.Persistence.Projections;

public class DfReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public decimal Discount { get; set; }
    public decimal Interest { get; set; }
    public DateOnly StartDate { get; set; }
    public string UpdatedBy { get; set; } = default!;
    public DateTime UpdatedAt { get; set; }
}
