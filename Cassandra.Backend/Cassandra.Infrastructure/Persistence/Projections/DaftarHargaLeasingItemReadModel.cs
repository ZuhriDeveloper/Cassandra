namespace Cassandra.Infrastructure.Persistence.Projections;

public class DaftarHargaLeasingItemReadModel
{
    public Guid DaftarHargaLeasingId { get; set; }
    public Guid GrupTipeMotorId { get; set; }
    public decimal Subsidi { get; set; }
    public decimal Incentive { get; set; }
    public decimal LainLain { get; set; }
}
