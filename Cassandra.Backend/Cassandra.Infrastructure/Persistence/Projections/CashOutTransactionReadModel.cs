namespace Cassandra.Infrastructure.Persistence.Projections;

public class CashOutTransactionReadModel
{
    public Guid     Id              { get; set; }
    public Guid     DealerId        { get; set; }
    public string   TransactionType { get; set; } = default!;
    public Guid?    SoId            { get; set; }
    public Guid?    SoReturId       { get; set; }
    public decimal  Amount          { get; set; }
    public decimal  DfAmount        { get; set; }
    public int      TotalHariDf     { get; set; }
    public DateTime PaymentDate     { get; set; }
    public string   PaymentMethod   { get; set; } = default!;
    public string   FInvoiceId      { get; set; } = default!;
    public bool     IsClosed        { get; set; }
    public string   CreatedBy       { get; set; } = default!;
    public DateTime CreatedAt       { get; set; }
}
