using Cassandra.Domain.Common;
using Cassandra.Domain.GlobalLeasing.Events;

namespace Cassandra.Domain.GlobalLeasing;

public class GlobalLeasing : AggregateRoot<GlobalLeasingId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string? Fax { get; private set; }
    public string Contact { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private GlobalLeasing() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static GlobalLeasing Create(
        string code, string name, string phone, string? fax,
        string contact, string address, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode global leasing tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama global leasing tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Telepon global leasing tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(contact))
            throw new DomainException("Kontak global leasing tidak boleh kosong.");

        var gl = new GlobalLeasing();
        gl.Raise(new GlobalLeasingCreated(
            GlobalLeasingId.New(),
            dealerId,
            code.Trim().ToUpper(),
            name.Trim(),
            phone.Trim(),
            fax?.Trim(),
            contact.Trim(),
            address?.Trim() ?? string.Empty,
            createdBy,
            DateTime.UtcNow));
        return gl;
    }

    public static GlobalLeasing Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var gl = new GlobalLeasing();
        gl.Load(events);
        return gl;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, string phone, string? fax, string contact, string address, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama global leasing tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Telepon global leasing tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(contact))
            throw new DomainException("Kontak global leasing tidak boleh kosong.");

        var trimName = name.Trim();
        var trimPhone = phone.Trim();
        var trimFax = fax?.Trim();
        var trimContact = contact.Trim();
        var trimAddress = address?.Trim() ?? string.Empty;

        if (Name == trimName && Phone == trimPhone && Fax == trimFax
            && Contact == trimContact && Address == trimAddress)
            return;

        Raise(new GlobalLeasingUpdated(Id, trimName, trimPhone, trimFax, trimContact, trimAddress, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Global leasing sudah aktif.");

        Raise(new GlobalLeasingActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Global leasing sudah tidak aktif.");

        Raise(new GlobalLeasingDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case GlobalLeasingCreated e:
                Id = e.GlobalLeasingId;
                DealerId = e.DealerId;
                Code = e.Code;
                Name = e.Name;
                Phone = e.Phone;
                Fax = e.Fax;
                Contact = e.Contact;
                Address = e.Address;
                IsActive = true;
                break;

            case GlobalLeasingUpdated e:
                Name = e.Name;
                Phone = e.Phone;
                Fax = e.Fax;
                Contact = e.Contact;
                Address = e.Address;
                break;

            case GlobalLeasingActivated:
                IsActive = true;
                break;

            case GlobalLeasingDeactivated:
                IsActive = false;
                break;
        }
    }
}
