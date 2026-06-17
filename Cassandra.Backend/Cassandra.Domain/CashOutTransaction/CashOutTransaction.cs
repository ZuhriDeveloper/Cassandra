using Cassandra.Domain.CashOutTransaction.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Domain.CashOutTransaction;

public class CashOutTransaction : AggregateRoot<CashOutTransactionId>
{
    // ── Transaction type constants ────────────────────────────────────────────
    public const string FSO_CASH       = "FSO_CASH";
    public const string FSO_DF         = "FSO_DF";
    public const string FSO_RETUR_CASH = "FSO_RETUR_CASH";
    public const string FSO_RETUR_DF   = "FSO_RETUR_DF";

    public Guid     DealerId        { get; private set; }
    public string   TransactionType { get; private set; } = default!;
    public Guid?    SoId            { get; private set; }
    public Guid?    SoReturId       { get; private set; }
    public decimal  Amount          { get; private set; }
    public decimal  DfAmount        { get; private set; }
    public int      TotalHariDf     { get; private set; }
    public DateTime PaymentDate     { get; private set; }
    public string   PaymentMethod   { get; private set; } = default!;
    public string   FInvoiceId      { get; private set; } = default!;
    public bool     IsClosed        { get; private set; }

    private CashOutTransaction() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static CashOutTransaction Create(
        CashOutTransactionId id,
        string transactionType,
        Guid? soId,
        Guid? soReturId,
        decimal amount,
        decimal dfAmount,
        int totalHariDf,
        DateTime paymentDate,
        string paymentMethod,
        string fInvoiceId,
        string createdBy,
        Guid dealerId)
    {
        if (amount <= 0)
            throw new DomainException("Jumlah pembayaran harus lebih dari nol.");
        if (string.IsNullOrWhiteSpace(transactionType))
            throw new DomainException("Tipe transaksi tidak boleh kosong.");

        var cashOut = new CashOutTransaction();
        cashOut.Raise(new CashOutTransactionCreated(
            id,
            dealerId,
            transactionType,
            soId,
            soReturId,
            amount,
            dfAmount,
            totalHariDf,
            paymentDate,
            paymentMethod,
            fInvoiceId,
            createdBy,
            DateTime.UtcNow));
        return cashOut;
    }

    public static CashOutTransaction Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var cashOut = new CashOutTransaction();
        cashOut.Load(events);
        return cashOut;
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        if (domainEvent is CashOutTransactionCreated e)
        {
            Id              = e.Id;
            DealerId        = e.DealerId;
            TransactionType = e.TransactionType;
            SoId            = e.SoId;
            SoReturId       = e.SoReturId;
            Amount          = e.Amount;
            DfAmount        = e.DfAmount;
            TotalHariDf     = e.TotalHariDf;
            PaymentDate     = e.PaymentDate;
            PaymentMethod   = e.PaymentMethod;
            FInvoiceId      = e.FInvoiceId;
            IsClosed        = true;
        }
    }
}
