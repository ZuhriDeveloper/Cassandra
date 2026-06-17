namespace Cassandra.Infrastructure.Persistence.Projections;

public class StnkReadModel
{
    public Guid     Id                    { get; set; }
    public Guid     DealerId              { get; set; }
    public Guid     RegistrasiPenjualanId { get; set; }
    public string   Status                { get; set; } = default!;
    public DateOnly FakturDate            { get; set; }
    public string   FakturName            { get; set; } = default!;
    public string   FakturAddress         { get; set; } = default!;
    public DateOnly? ProcessDate          { get; set; }
    public Guid?    BiroId                { get; set; }
    public string?  InvoiceNumber         { get; set; }
    public string?  PlateNumber           { get; set; }
    public string?  StnkNumber            { get; set; }
    public decimal  StnkCost              { get; set; }
    public decimal  ProgressiveCost       { get; set; }
    public decimal  NoticeCost            { get; set; }
    public DateOnly? ReceiveDate          { get; set; }
    public DateOnly? HandoverDate         { get; set; }
    public string?  StnkReceiver          { get; set; }
    public string?  Region                { get; set; }
    public decimal  BbnCost               { get; set; }
    public decimal  PnbpCost              { get; set; }
    public decimal  AdminCost             { get; set; }
    public decimal  OtherCost             { get; set; }
    public decimal  ServiceCost           { get; set; }
    public decimal  PphCost               { get; set; }
    public bool?    IsInvoiceValid        { get; set; }
    public string   CreatedBy             { get; set; } = default!;
    public DateTime CreatedAt             { get; set; }
    public string?  UpdatedBy             { get; set; }
    public DateTime? UpdatedAt            { get; set; }
}
