namespace Cassandra.Infrastructure.Persistence.Projections;

public class BpkbReadModel
{
    public Guid     Id                    { get; set; }
    public Guid     DealerId              { get; set; }
    public Guid     RegistrasiPenjualanId { get; set; }
    public Guid     StnkId               { get; set; }
    public string   Status               { get; set; } = default!;
    public DateOnly RequestDate          { get; set; }
    public string?  BpkbNumber           { get; set; }
    public string?  BookNumber           { get; set; }
    public DateOnly? ReceiveDate         { get; set; }
    public DateOnly? HandoverDate        { get; set; }
    public string?  Receiver             { get; set; }
    public string   CreatedBy            { get; set; } = default!;
    public DateTime CreatedAt            { get; set; }
    public string?  UpdatedBy            { get; set; }
    public DateTime? UpdatedAt           { get; set; }
}
