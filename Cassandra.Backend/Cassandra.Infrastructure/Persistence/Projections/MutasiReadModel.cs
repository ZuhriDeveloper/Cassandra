namespace Cassandra.Infrastructure.Persistence.Projections;

public class MutasiReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string MutasiNumber { get; set; } = default!;
    public DateOnly MutasiDate { get; set; }
    public Guid SourceKiosId { get; set; }
    public Guid DestinationKiosId { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

public class MutasiItemReadModel
{
    public Guid MutasiId { get; set; }
    public string NoMesin { get; set; } = default!;
}

public class MutasiKelengkapanReadModel
{
    public Guid MutasiId { get; set; }
    public string KelengkapanName { get; set; } = default!;
    public int Qty { get; set; }
}
