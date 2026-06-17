namespace Cassandra.Infrastructure.Persistence.Projections;

public class PengirimanMotorReadModel
{
    public Guid     Id                    { get; set; }
    public Guid     DealerId              { get; set; }
    public Guid     RegistrasiPenjualanId { get; set; }
    public string   NoMesin               { get; set; } = default!;
    public Guid     Driver1Id             { get; set; }
    public Guid?    Driver2Id             { get; set; }
    public DateOnly DeliveryDate          { get; set; }
    public string?  Zona                  { get; set; }
    public string   CreatedBy             { get; set; } = default!;
    public DateTime CreatedAt             { get; set; }
}
