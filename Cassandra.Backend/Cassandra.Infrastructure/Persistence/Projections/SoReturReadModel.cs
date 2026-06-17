namespace Cassandra.Infrastructure.Persistence.Projections;

public class SoReturReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string ReturNumber { get; set; } = default!;
    public Guid SoId { get; set; }
    public DateOnly ReturDate { get; set; }
    public string Reason { get; set; } = default!;
    public decimal Total { get; set; }
    public decimal PPn { get; set; }
    public decimal TotalAmount { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

public class SoReturItemReadModel
{
    public Guid SoReturId { get; set; }
    public Guid TipeMotorId { get; set; }
    public Guid WarnaId { get; set; }
    public int Qty { get; set; }
    public decimal NettPrice { get; set; }
}
