namespace Cassandra.Infrastructure.Persistence.Projections;

/// <summary>
/// Read model for the dealer registry. This is platform-level data, visible to a SuperAdmin
/// across all dealers — deliberately NOT subject to any per-dealer query filter.
/// </summary>
public class DealerReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
