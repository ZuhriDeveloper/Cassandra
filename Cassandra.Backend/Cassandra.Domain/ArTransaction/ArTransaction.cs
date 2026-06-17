using Cassandra.Domain.ArTransaction.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Domain.ArTransaction;

public record ArPaymentEntry(
    int PaymentNo,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string FInvoiceId,
    string CreatedBy);

public class ArTransaction : AggregateRoot<ArTransactionId>
{
    // ── Transaction type constants ────────────────────────────────────────────
    public const string PENJUALAN        = "PENJUALAN";
    public const string AMBIL_UANG       = "AMBIL_UANG";
    public const string PENJUALAN_CREDIT = "PENJUALAN_CREDIT";
    public const string SO_RETUR         = "SO_RETUR";

    public Guid    DealerId          { get; private set; }
    public string  TransactionType   { get; private set; } = default!;
    public Guid?   ReferenceId       { get; private set; }
    public string  ReferenceNumber   { get; private set; } = default!;
    public decimal TotalAmount       { get; private set; }
    public decimal RemainingAmount   { get; private set; }
    public bool    IsClosed          { get; private set; }

    private readonly List<ArPaymentEntry> _payments = [];
    public IReadOnlyList<ArPaymentEntry> Payments => _payments.AsReadOnly();

    private ArTransaction() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static ArTransaction Create(
        ArTransactionId id,
        string transactionType,
        Guid? referenceId,
        string referenceNumber,
        decimal totalAmount,
        string createdBy,
        Guid dealerId)
    {
        if (totalAmount <= 0)
            throw new DomainException("Total amount harus lebih dari nol.");
        if (string.IsNullOrWhiteSpace(transactionType))
            throw new DomainException("Tipe transaksi tidak boleh kosong.");

        var ar = new ArTransaction();
        ar.Raise(new ArTransactionCreated(
            id,
            dealerId,
            transactionType,
            referenceId,
            referenceNumber,
            totalAmount,
            createdBy,
            DateTime.UtcNow));
        return ar;
    }

    public static ArTransaction Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var ar = new ArTransaction();
        ar.Load(events);
        return ar;
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
            throw new DomainException("Transaksi AR sudah ditutup.");
        if (amount <= 0)
            throw new DomainException("Jumlah pembayaran harus lebih dari nol.");

        Raise(new ArTransactionPaymentRecorded(Id, paymentNo, amount, paymentDate, paymentMethod, fInvoiceId, createdBy, occurredAt));

        if (RemainingAmount <= 0)
            Raise(new ArTransactionClosed(Id, createdBy, occurredAt));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case ArTransactionCreated e:
                Id              = e.Id;
                DealerId        = e.DealerId;
                TransactionType = e.TransactionType;
                ReferenceId     = e.ReferenceId;
                ReferenceNumber = e.ReferenceNumber;
                TotalAmount     = e.TotalAmount;
                RemainingAmount = e.TotalAmount;
                IsClosed        = false;
                _payments.Clear();
                break;

            case ArTransactionPaymentRecorded e:
                _payments.Add(new ArPaymentEntry(e.PaymentNo, e.Amount, e.PaymentDate, e.PaymentMethod, e.FInvoiceId, e.CreatedBy));
                RemainingAmount -= e.Amount;
                break;

            case ArTransactionClosed:
                IsClosed = true;
                break;
        }
    }
}
