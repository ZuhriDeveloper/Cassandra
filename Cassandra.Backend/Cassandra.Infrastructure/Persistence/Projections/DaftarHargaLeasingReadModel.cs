namespace Cassandra.Infrastructure.Persistence.Projections;

public class DaftarHargaLeasingReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string Name { get; set; } = default!;
    public Guid GlobalLeasingId { get; set; }
    public Guid GrupTenorId { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
