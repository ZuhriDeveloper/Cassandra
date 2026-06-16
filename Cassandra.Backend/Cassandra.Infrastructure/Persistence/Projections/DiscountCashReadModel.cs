namespace Cassandra.Infrastructure.Persistence.Projections;

public class DiscountCashReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public Guid TipeMotorId { get; set; }
    public decimal DirectDiscount { get; set; }
    public decimal ChannelDiscount { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
