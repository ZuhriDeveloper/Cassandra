using Cassandra.Domain.Common;
using Cassandra.Domain.So.Events;

namespace Cassandra.Domain.So;

public class So : AggregateRoot<SoId>
{
    public Guid DealerId { get; private set; }
    public string SoNumber { get; private set; } = default!;
    public DateOnly SoDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public string PaymentType { get; private set; } = default!;
    public Guid MetodeKeuanganId { get; private set; }
    public int QtyUnit { get; private set; }
    public decimal Total { get; private set; }
    public decimal Subsidi { get; private set; }
    public decimal CashDiscount { get; private set; }
    public decimal PPn { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal Df { get; private set; }
    public string Status { get; private set; } = default!;
    public bool IsDeleted { get; private set; }
    public IReadOnlyList<SoItemValue> Items { get; private set; } = [];

    private So() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static So Create(
        string soNumber,
        DateOnly soDate,
        DateOnly dueDate,
        string paymentType,
        Guid metodeKeuanganId,
        decimal total,
        decimal subsidi,
        decimal cashDiscount,
        decimal ppn,
        decimal totalAmount,
        decimal df,
        int qtyUnit,
        IReadOnlyList<SoItemValue> items,
        string createdBy,
        Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(soNumber))
            throw new DomainException("Nomor SO tidak boleh kosong.");
        if (items == null || items.Count == 0)
            throw new DomainException("SO harus memiliki minimal satu item.");
        if (qtyUnit <= 0)
            throw new DomainException("Jumlah unit harus lebih dari nol.");
        if (paymentType != SoPaymentType.CASH && paymentType != SoPaymentType.DF)
            throw new DomainException($"Tipe pembayaran tidak valid: '{paymentType}'. Harus CASH atau DF.");

        var so = new So();
        so.Raise(new SoCreated(
            SoId.New(),
            dealerId,
            soNumber.Trim().ToUpper(),
            soDate,
            dueDate,
            paymentType,
            metodeKeuanganId,
            qtyUnit,
            total,
            subsidi,
            cashDiscount,
            ppn,
            totalAmount,
            df,
            items,
            createdBy,
            DateTime.UtcNow));
        return so;
    }

    public static So Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var so = new So();
        so.Load(events);
        return so;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void ChangeStatus(string status, string updatedBy)
    {
        Raise(new SoStatusChanged(Id, status, updatedBy, DateTime.UtcNow));
    }

    public void Delete(string deletedBy)
    {
        if (Status != SoStatus.AKTIF)
            throw new DomainException("SO hanya dapat dihapus jika berstatus AKTIF.");

        Raise(new SoDeleted(Id, deletedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SoCreated e:
                Id = e.Id;
                DealerId = e.DealerId;
                SoNumber = e.SoNumber;
                SoDate = e.SoDate;
                DueDate = e.DueDate;
                PaymentType = e.PaymentType;
                MetodeKeuanganId = e.MetodeKeuanganId;
                QtyUnit = e.QtyUnit;
                Total = e.Total;
                Subsidi = e.Subsidi;
                CashDiscount = e.CashDiscount;
                PPn = e.PPn;
                TotalAmount = e.TotalAmount;
                Df = e.Df;
                Items = e.Items;
                Status = SoStatus.AKTIF;
                IsDeleted = false;
                break;

            case SoStatusChanged e:
                Status = e.Status;
                break;

            case SoDeleted:
                IsDeleted = true;
                break;
        }
    }
}
