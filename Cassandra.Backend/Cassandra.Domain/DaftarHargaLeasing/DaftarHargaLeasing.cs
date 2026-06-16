using Cassandra.Domain.Common;
using Cassandra.Domain.DaftarHargaLeasing.Events;

namespace Cassandra.Domain.DaftarHargaLeasing;

public class DaftarHargaLeasing : AggregateRoot<DaftarHargaLeasingId>
{
    public Guid DealerId { get; private set; }
    public string Name { get; private set; } = default!;
    public Guid GlobalLeasingId { get; private set; }
    public Guid GrupTenorId { get; private set; }
    public bool IsActive { get; private set; }

    private List<DaftarHargaLeasingItem> _items = new();
    public IReadOnlyList<DaftarHargaLeasingItem> Items => _items.AsReadOnly();

    private DaftarHargaLeasing() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static DaftarHargaLeasing Create(
        string name, Guid globalLeasingId, Guid grupTenorId, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama daftar harga leasing tidak boleh kosong.");
        if (globalLeasingId == Guid.Empty)
            throw new DomainException("Global leasing harus dipilih.");
        if (grupTenorId == Guid.Empty)
            throw new DomainException("Grup tenor harus dipilih.");

        var dhl = new DaftarHargaLeasing();
        dhl.Raise(new DaftarHargaLeasingCreated(
            DaftarHargaLeasingId.New(),
            dealerId,
            name.Trim(),
            globalLeasingId,
            grupTenorId,
            createdBy,
            DateTime.UtcNow));
        return dhl;
    }

    public static DaftarHargaLeasing Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var dhl = new DaftarHargaLeasing();
        dhl.Load(events);
        return dhl;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, Guid globalLeasingId, Guid grupTenorId, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama daftar harga leasing tidak boleh kosong.");
        if (globalLeasingId == Guid.Empty)
            throw new DomainException("Global leasing harus dipilih.");
        if (grupTenorId == Guid.Empty)
            throw new DomainException("Grup tenor harus dipilih.");

        var trimName = name.Trim();
        if (Name == trimName && GlobalLeasingId == globalLeasingId && GrupTenorId == grupTenorId) return;

        Raise(new DaftarHargaLeasingUpdated(Id, trimName, globalLeasingId, grupTenorId, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Daftar harga leasing sudah aktif.");

        Raise(new DaftarHargaLeasingActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Daftar harga leasing sudah tidak aktif.");

        Raise(new DaftarHargaLeasingDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void SetItems(IReadOnlyList<DaftarHargaLeasingItem> items, string updatedBy)
    {
        Raise(new DaftarHargaLeasingItemsSet(Id, items, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case DaftarHargaLeasingCreated e:
                Id = e.DaftarHargaLeasingId;
                DealerId = e.DealerId;
                Name = e.Name;
                GlobalLeasingId = e.GlobalLeasingId;
                GrupTenorId = e.GrupTenorId;
                IsActive = true;
                break;

            case DaftarHargaLeasingUpdated e:
                Name = e.Name;
                GlobalLeasingId = e.GlobalLeasingId;
                GrupTenorId = e.GrupTenorId;
                break;

            case DaftarHargaLeasingActivated:
                IsActive = true;
                break;

            case DaftarHargaLeasingDeactivated:
                IsActive = false;
                break;

            case DaftarHargaLeasingItemsSet e:
                _items = [.. e.Items];
                break;
        }
    }
}
