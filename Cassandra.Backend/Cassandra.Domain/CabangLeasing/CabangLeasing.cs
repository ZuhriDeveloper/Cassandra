using Cassandra.Domain.Common;
using Cassandra.Domain.CabangLeasing.Events;

namespace Cassandra.Domain.CabangLeasing;

public class CabangLeasing : AggregateRoot<CabangLeasingId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? Fax { get; private set; }
    public string? Contact { get; private set; }
    public Guid GlobalLeasingId { get; private set; }
    public bool IsActive { get; private set; }

    private CabangLeasing() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static CabangLeasing Create(
        string code, string name, string? phone, string? fax, string? contact,
        Guid globalLeasingId, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode cabang leasing tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama cabang leasing tidak boleh kosong.");
        if (globalLeasingId == Guid.Empty)
            throw new DomainException("Global leasing harus dipilih.");

        var cl = new CabangLeasing();
        cl.Raise(new CabangLeasingCreated(
            CabangLeasingId.New(),
            dealerId,
            code.Trim().ToUpper(),
            name.Trim(),
            phone?.Trim(),
            fax?.Trim(),
            contact?.Trim(),
            globalLeasingId,
            createdBy,
            DateTime.UtcNow));
        return cl;
    }

    public static CabangLeasing Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var cl = new CabangLeasing();
        cl.Load(events);
        return cl;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, string? phone, string? fax, string? contact, Guid globalLeasingId, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama cabang leasing tidak boleh kosong.");
        if (globalLeasingId == Guid.Empty)
            throw new DomainException("Global leasing harus dipilih.");

        var trimName = name.Trim();
        var trimPhone = phone?.Trim();
        var trimFax = fax?.Trim();
        var trimContact = contact?.Trim();

        if (Name == trimName && Phone == trimPhone && Fax == trimFax
            && Contact == trimContact && GlobalLeasingId == globalLeasingId)
            return;

        Raise(new CabangLeasingUpdated(Id, trimName, trimPhone, trimFax, trimContact, globalLeasingId, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Cabang leasing sudah aktif.");

        Raise(new CabangLeasingActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Cabang leasing sudah tidak aktif.");

        Raise(new CabangLeasingDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case CabangLeasingCreated e:
                Id = e.CabangLeasingId;
                DealerId = e.DealerId;
                Code = e.Code;
                Name = e.Name;
                Phone = e.Phone;
                Fax = e.Fax;
                Contact = e.Contact;
                GlobalLeasingId = e.GlobalLeasingId;
                IsActive = true;
                break;

            case CabangLeasingUpdated e:
                Name = e.Name;
                Phone = e.Phone;
                Fax = e.Fax;
                Contact = e.Contact;
                GlobalLeasingId = e.GlobalLeasingId;
                break;

            case CabangLeasingActivated:
                IsActive = true;
                break;

            case CabangLeasingDeactivated:
                IsActive = false;
                break;
        }
    }
}
