namespace Cassandra.Infrastructure.Persistence.Projections;

public class ApTransactionReadModel
{
    public Guid     Id              { get; set; }
    public Guid     DealerId        { get; set; }
    public string   TransactionType { get; set; } = default!;
    public Guid     StnkId          { get; set; }
    public string   NoRangka        { get; set; } = default!;
    public decimal  TotalAmount     { get; set; }
    public decimal  RemainingAmount { get; set; }
    public bool     IsClosed        { get; set; }
    public string   CreatedBy       { get; set; } = default!;
    public DateTime CreatedAt       { get; set; }
    public List<ApPaymentEntryReadModel> Payments { get; set; } = [];
}

public class ApPaymentEntryReadModel
{
    public int      PaymentNo     { get; set; }
    public decimal  Amount        { get; set; }
    public DateTime PaymentDate   { get; set; }
    public string   PaymentMethod { get; set; } = default!;
    public string   FInvoiceId    { get; set; } = default!;
    public string   CreatedBy     { get; set; } = default!;
}
