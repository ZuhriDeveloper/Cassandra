using Cassandra.Domain.Common;
using Cassandra.Domain.Discount.Events;

namespace Cassandra.Domain.Discount;

public class Discount : AggregateRoot<DiscountId>
{
    public Guid DealerId { get; private set; }
    public Guid DaftarHargaLeasingId { get; private set; }
    public string Level { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private List<DiscountLineItem> _items = new();
    public IReadOnlyList<DiscountLineItem> Items => _items.AsReadOnly();

    private Discount() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Discount Create(Guid daftarHargaLeasingId, string level, string createdBy, Guid dealerId)
    {
        if (daftarHargaLeasingId == Guid.Empty)
            throw new DomainException("Daftar harga leasing harus dipilih.");
        if (string.IsNullOrWhiteSpace(level))
            throw new DomainException("Level discount tidak boleh kosong.");

        var discount = new Discount();
        discount.Raise(new DiscountCreated(
            DiscountId.New(),
            dealerId,
            daftarHargaLeasingId,
            level.Trim(),
            createdBy,
            DateTime.UtcNow));
        return discount;
    }

    public static Discount Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var discount = new Discount();
        discount.Load(events);
        return discount;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(Guid daftarHargaLeasingId, string level, string updatedBy)
    {
        if (daftarHargaLeasingId == Guid.Empty)
            throw new DomainException("Daftar harga leasing harus dipilih.");
        if (string.IsNullOrWhiteSpace(level))
            throw new DomainException("Level discount tidak boleh kosong.");

        var trimLevel = level.Trim();
        if (DaftarHargaLeasingId == daftarHargaLeasingId && Level == trimLevel) return;

        Raise(new DiscountUpdated(Id, daftarHargaLeasingId, trimLevel, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Discount sudah aktif.");

        Raise(new DiscountActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Discount sudah tidak aktif.");

        Raise(new DiscountDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void SetItems(IReadOnlyList<DiscountLineItem> items, string updatedBy)
    {
        Raise(new DiscountItemsSet(Id, items, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case DiscountCreated e:
                Id = e.DiscountId;
                DealerId = e.DealerId;
                DaftarHargaLeasingId = e.DaftarHargaLeasingId;
                Level = e.Level;
                IsActive = true;
                break;

            case DiscountUpdated e:
                DaftarHargaLeasingId = e.DaftarHargaLeasingId;
                Level = e.Level;
                break;

            case DiscountActivated:
                IsActive = true;
                break;

            case DiscountDeactivated:
                IsActive = false;
                break;

            case DiscountItemsSet e:
                _items = [.. e.Items];
                break;
        }
    }
}
