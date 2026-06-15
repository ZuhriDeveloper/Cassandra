namespace Cassandra.Infrastructure.Persistence.Projections;

/// <summary>
/// Read model for jabatan (positions) scoped to a single dealer.
/// Subject to a global query filter on DealerId in <see cref="AppDbContext"/>.
/// </summary>
public class JabatanReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
