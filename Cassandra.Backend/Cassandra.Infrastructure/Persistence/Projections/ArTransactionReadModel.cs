namespace Cassandra.Infrastructure.Persistence.Projections;

public class ArTransactionReadModel
{
    public Guid     Id              { get; set; }
    public Guid     DealerId        { get; set; }
    public string   TransactionType { get; set; } = default!;
    public Guid?    ReferenceId     { get; set; }
    public string   ReferenceNumber { get; set; } = default!;
    public decimal  TotalAmount     { get; set; }
    public decimal  RemainingAmount { get; set; }
    public bool     IsClosed        { get; set; }
    public string   CreatedBy       { get; set; } = default!;
    public DateTime CreatedAt       { get; set; }
    public List<ArPaymentEntryReadModel> Payments { get; set; } = [];
}

public class ArPaymentEntryReadModel
{
    public int      PaymentNo     { get; set; }
    public decimal  Amount        { get; set; }
    public DateTime PaymentDate   { get; set; }
    public string   PaymentMethod { get; set; } = default!;
    public string   FInvoiceId    { get; set; } = default!;
    public string   CreatedBy     { get; set; } = default!;
}
