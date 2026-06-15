namespace Cassandra.Infrastructure.Persistence.Projections;

public class KaryawanReadModel
{
    public Guid      Id         { get; set; }
    public Guid      DealerId   { get; set; }
    public string    Name       { get; set; } = default!;
    public string    Email      { get; set; } = default!;
    public string    KtpNumber  { get; set; } = default!;
    public string    Gender     { get; set; } = default!;
    public DateOnly  HireDate   { get; set; }
    public DateOnly? ResignDate { get; set; }
    public string    Phone      { get; set; } = default!;
    public string?   PhoneAlt   { get; set; }
    public string    Address    { get; set; } = default!;
    public decimal   SalesLimit { get; set; }
    public Guid      JabatanId  { get; set; }
    public bool      IsActive   { get; set; }
    public string    CreatedBy  { get; set; } = default!;
    public DateTime  CreatedAt  { get; set; }
}
