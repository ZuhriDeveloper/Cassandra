namespace Cassandra.Infrastructure.Persistence.Projections;

public class PelanggaranWilayahReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string AreaCode { get; set; } = default!;
    public decimal ExtraFee { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
