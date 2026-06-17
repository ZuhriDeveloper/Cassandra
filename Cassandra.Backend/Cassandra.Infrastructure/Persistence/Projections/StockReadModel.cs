namespace Cassandra.Infrastructure.Persistence.Projections;

public class StockReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string NoMesin { get; set; } = default!;
    public string NoRangka { get; set; } = default!;
    public Guid TipeMotorId { get; set; }
    public Guid WarnaId { get; set; }
    public Guid KiosId { get; set; }
    public string SuratJalanId { get; set; } = default!;
    public DateOnly SuratJalanDate { get; set; }
    public Guid SoId { get; set; }
    public string AssemblyYear { get; set; } = default!;
    public string Status { get; set; } = default!;
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
