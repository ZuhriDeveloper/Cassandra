using Cassandra.Domain.Common;
using Cassandra.Domain.SoRetur.Events;

namespace Cassandra.Domain.SoRetur;

public class SoRetur : AggregateRoot<SoReturId>
{
    public Guid DealerId { get; private set; }
    public string ReturNumber { get; private set; } = default!;
    public Guid SoId { get; private set; }
    public DateOnly ReturDate { get; private set; }
    public string Reason { get; private set; } = default!;
    public decimal Total { get; private set; }
    public decimal PPn { get; private set; }
    public decimal TotalAmount { get; private set; }
    public IReadOnlyList<SoReturItemValue> Items { get; private set; } = [];

    private SoRetur() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static SoRetur Create(
        string returNumber,
        Guid soId,
        DateOnly returDate,
        string reason,
        IReadOnlyList<SoReturItemValue> items,
        string createdBy,
        Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(returNumber))
            throw new DomainException("Nomor retur tidak boleh kosong.");
        if (soId == Guid.Empty)
            throw new DomainException("SO ID tidak valid.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Alasan retur tidak boleh kosong.");
        if (items == null || items.Count == 0)
            throw new DomainException("Retur harus memiliki minimal satu item.");

        var total = items.Sum(i => i.Qty * i.NettPrice);
        var ppn = total * 0.1m;
        var totalAmount = total + ppn;

        var retur = new SoRetur();
        retur.Raise(new SoReturCreated(
            SoReturId.New(),
            dealerId,
            returNumber.Trim().ToUpper(),
            soId,
            returDate,
            reason.Trim(),
            total,
            ppn,
            totalAmount,
            items,
            createdBy,
            DateTime.UtcNow));
        return retur;
    }

    public static SoRetur Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var retur = new SoRetur();
        retur.Load(events);
        return retur;
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        if (domainEvent is SoReturCreated e)
        {
            Id = e.Id;
            DealerId = e.DealerId;
            ReturNumber = e.ReturNumber;
            SoId = e.SoId;
            ReturDate = e.ReturDate;
            Reason = e.Reason;
            Total = e.Total;
            PPn = e.PPn;
            TotalAmount = e.TotalAmount;
            Items = e.Items;
        }
    }
}
