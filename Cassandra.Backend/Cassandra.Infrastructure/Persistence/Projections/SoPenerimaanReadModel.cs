namespace Cassandra.Infrastructure.Persistence.Projections;

public class SoPenerimaanReadModel
{
    public Guid Id { get; set; }
    public Guid DealerId { get; set; }
    public string SuratJalanId { get; set; } = default!;
    public DateOnly SuratJalanDate { get; set; }
    public Guid SoId { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}

public class SoPenerimaanItemMotorReadModel
{
    public Guid SoPenerimaanId { get; set; }
    public Guid TipeMotorId { get; set; }
    public Guid WarnaId { get; set; }
    public string NoMesin { get; set; } = default!;
    public string NoRangka { get; set; } = default!;
    public Guid KiosId { get; set; }
    public string AssemblyYear { get; set; } = default!;
}

public class SoPenerimaanItemKelengkapanReadModel
{
    public Guid SoPenerimaanId { get; set; }
    public Guid KelengkapanId { get; set; }
    public int Qty { get; set; }
    public string? Notes { get; set; }
}
