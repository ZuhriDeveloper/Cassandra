namespace Cassandra.Infrastructure.Persistence.Projections;

public class BiayaBiroJasaReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public Guid SamsatId { get; set; }
    public Guid BiroId { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
