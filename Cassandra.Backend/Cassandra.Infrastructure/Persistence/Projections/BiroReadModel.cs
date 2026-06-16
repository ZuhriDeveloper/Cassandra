namespace Cassandra.Infrastructure.Persistence.Projections;

public class BiroReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Pic { get; set; }
    public string? Address { get; set; }
    public decimal PphRate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
