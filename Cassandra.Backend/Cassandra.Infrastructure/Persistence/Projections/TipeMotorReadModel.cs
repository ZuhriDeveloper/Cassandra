namespace Cassandra.Infrastructure.Persistence.Projections;

public class TipeMotorReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string Code { get; set; } = default!;
    public Guid GrupTipeMotorId { get; set; }
    public string ShortName { get; set; } = default!;
    public string ProductCode { get; set; } = default!;
    public string WmsCode { get; set; } = default!;
    public string AhmCode { get; set; } = default!;
    public string EngineNumberFormat { get; set; } = default!;
    public string ChassisNumberFormat { get; set; } = default!;
    public decimal NettPrice { get; set; }
    public decimal OrJakarta { get; set; }
    public decimal OrTangerang { get; set; }
    public decimal BbnJakarta { get; set; }
    public decimal BbnTangerang { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
