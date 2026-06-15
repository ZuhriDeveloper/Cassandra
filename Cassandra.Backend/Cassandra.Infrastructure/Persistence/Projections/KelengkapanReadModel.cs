namespace Cassandra.Infrastructure.Persistence.Projections;

public class KelengkapanReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
