namespace Cassandra.Infrastructure.Persistence.Projections;

public class SoReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string SoNumber { get; set; } = default!;
    public DateOnly SoDate { get; set; }
    public DateOnly DueDate { get; set; }
    public string PaymentType { get; set; } = default!;
    public Guid MetodeKeuanganId { get; set; }
    public int QtyUnit { get; set; }
    public decimal Total { get; set; }
    public decimal Subsidi { get; set; }
    public decimal CashDiscount { get; set; }
    public decimal PPn { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Df { get; set; }
    public string Status { get; set; } = default!;
    public bool IsDeleted { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SoItemReadModel
{
    public Guid SoId { get; set; }
    public Guid TipeMotorId { get; set; }
    public Guid WarnaId { get; set; }
    public int Qty { get; set; }
    public decimal NettPrice { get; set; }
}
