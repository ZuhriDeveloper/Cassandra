using Cassandra.Domain.Common;
using Cassandra.Domain.Kios.Events;

namespace Cassandra.Domain.Kios;

public class Kios : AggregateRoot<KiosId>
{
    public Guid    DealerId      { get; private set; }
    public string  Code          { get; private set; } = default!;
    public string  Name          { get; private set; } = default!;
    public string  Phone         { get; private set; } = default!;
    public string? Fax           { get; private set; }
    public string  Address       { get; private set; } = default!;
    public Guid    PicKaryawanId { get; private set; }
    public decimal Limit         { get; private set; }
    public bool    IsActive      { get; private set; }

    private Kios() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Kios Create(
        string  code,
        string  name,
        string  phone,
        string? fax,
        string  address,
        Guid    picKaryawanId,
        decimal limit,
        string  createdBy,
        Guid    dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode kios tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama kios tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Nomor telepon kios tidak boleh kosong.");
        if (limit < 0)
            throw new DomainException("Limit kios tidak boleh negatif.");

        var kios = new Kios();
        kios.Raise(new KiosCreated(
            KiosId.New(),
            dealerId,
            code.Trim().ToUpper(),
            name.Trim(),
            phone.Trim(),
            fax?.Trim(),
            address?.Trim() ?? string.Empty,
            picKaryawanId,
            limit,
            createdBy,
            DateTime.UtcNow));
        return kios;
    }

    public static Kios Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var kios = new Kios();
        kios.Load(events);
        return kios;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(
        string  name,
        string  phone,
        string? fax,
        string  address,
        Guid    picKaryawanId,
        string  updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama kios tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Nomor telepon kios tidak boleh kosong.");

        var trimName    = name.Trim();
        var trimPhone   = phone.Trim();
        var trimFax     = fax?.Trim();
        var trimAddress = address?.Trim() ?? string.Empty;

        if (Name == trimName && Phone == trimPhone && Fax == trimFax &&
            Address == trimAddress && PicKaryawanId == picKaryawanId)
            return;

        Raise(new KiosUpdated(Id, trimName, trimPhone, trimFax, trimAddress, picKaryawanId, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string activatedBy)
    {
        if (IsActive)
            throw new DomainException("Kios sudah aktif.");
        Raise(new KiosActivated(Id, activatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string deactivatedBy)
    {
        if (!IsActive)
            throw new DomainException("Kios sudah tidak aktif.");
        Raise(new KiosDeactivated(Id, deactivatedBy, DateTime.UtcNow));
    }

    public void SetLimit(decimal limit, string setBy)
    {
        if (limit < 0)
            throw new DomainException("Limit kios tidak boleh negatif.");
        Raise(new KiosLimitSet(Id, limit, setBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case KiosCreated e:
                Id            = e.KiosId;
                DealerId      = e.DealerId;
                Code          = e.Code;
                Name          = e.Name;
                Phone         = e.Phone;
                Fax           = e.Fax;
                Address       = e.Address;
                PicKaryawanId = e.PicKaryawanId;
                Limit         = e.Limit;
                IsActive      = true;
                break;

            case KiosUpdated e:
                Name          = e.Name;
                Phone         = e.Phone;
                Fax           = e.Fax;
                Address       = e.Address;
                PicKaryawanId = e.PicKaryawanId;
                break;

            case KiosActivated:
                IsActive = true;
                break;

            case KiosDeactivated:
                IsActive = false;
                break;

            case KiosLimitSet e:
                Limit = e.Limit;
                break;
        }
    }
}
