using Cassandra.Domain.ApTransaction.Events;
using Cassandra.Domain.ArTransaction;
using Cassandra.Domain.Common;

namespace Cassandra.Domain.ApTransaction;

public class ApTransaction : AggregateRoot<ApTransactionId>
{
    // ── Transaction type constants ────────────────────────────────────────────
    public const string STNK             = "STNK";
    public const string PPH_STNK         = "PPH_STNK";
    public const string PROGRESSIVE_STNK = "PROGRESSIVE_STNK";

    public Guid    DealerId        { get; private set; }
    public string  TransactionType { get; private set; } = default!;
    public Guid    StnkId          { get; private set; }
    public string  NoRangka        { get; private set; } = default!;
    public decimal TotalAmount     { get; private set; }
    public decimal RemainingAmount { get; private set; }
    public bool    IsClosed        { get; private set; }

    private readonly List<ArPaymentEntry> _payments = [];
    public IReadOnlyList<ArPaymentEntry> Payments => _payments.AsReadOnly();

    private ApTransaction() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static ApTransaction Create(
        ApTransactionId id,
        string transactionType,
        Guid stnkId,
        string noRangka,
        decimal totalAmount,
        string createdBy,
        Guid dealerId)
    {
        if (totalAmount <= 0)
            throw new DomainException("Total amount harus lebih dari nol.");
        if (string.IsNullOrWhiteSpace(transactionType))
            throw new DomainException("Tipe transaksi tidak boleh kosong.");

        var ap = new ApTransaction();
        ap.Raise(new ApTransactionCreated(
            id,
            dealerId,
            transactionType,
            stnkId,
            noRangka,
            totalAmount,
            createdBy,
            DateTime.UtcNow));
        return ap;
    }

    public static ApTransaction Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var ap = new ApTransaction();
        ap.Load(events);
        return ap;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void RecordPayment(
        int paymentNo,
        decimal amount,
        DateTime paymentDate,
        string paymentMethod,
        string fInvoiceId,
        string createdBy,
        DateTime occurredAt)
    {
        if (IsClosed)
            throw new DomainException("Transaksi AP sudah ditutup.");
        if (amount <= 0)
            throw new DomainException("Jumlah pembayaran harus lebih dari nol.");

        Raise(new ApTransactionPaymentRecorded(Id, paymentNo, amount, paymentDate, paymentMethod, fInvoiceId, createdBy, occurredAt));

        if (RemainingAmount <= 0)
            Raise(new ApTransactionClosed(Id, createdBy, occurredAt));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case ApTransactionCreated e:
                Id              = e.Id;
                DealerId        = e.DealerId;
                TransactionType = e.TransactionType;
                StnkId          = e.StnkId;
                NoRangka        = e.NoRangka;
                TotalAmount     = e.TotalAmount;
                RemainingAmount = e.TotalAmount;
                IsClosed        = false;
                _payments.Clear();
                break;

            case ApTransactionPaymentRecorded e:
                _payments.Add(new ArPaymentEntry(e.PaymentNo, e.Amount, e.PaymentDate, e.PaymentMethod, e.FInvoiceId, e.CreatedBy));
                RemainingAmount -= e.Amount;
                break;

            case ApTransactionClosed:
                IsClosed = true;
                break;
        }
    }
}
