using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan.Events;

namespace Cassandra.Domain.Karyawan;

public class Karyawan : AggregateRoot<KaryawanId>
{
    public Guid      DealerId   { get; private set; }
    public string    Name       { get; private set; } = default!;
    public string    Email      { get; private set; } = default!;
    public string    KtpNumber  { get; private set; } = default!;
    public Gender    Gender     { get; private set; }
    public DateOnly  HireDate   { get; private set; }
    public DateOnly? ResignDate { get; private set; }
    public string    Phone      { get; private set; } = default!;
    public string?   PhoneAlt   { get; private set; }
    public string    Address    { get; private set; } = default!;
    public decimal   SalesLimit { get; private set; }
    public Guid      JabatanId  { get; private set; }
    public bool      IsActive   { get; private set; }

    private Karyawan() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Karyawan Create(
        string   name,
        string   email,
        string   ktpNumber,
        Gender   gender,
        DateOnly hireDate,
        string   phone,
        string?  phoneAlt,
        string   address,
        decimal  salesLimit,
        Guid     jabatanId,
        string   createdBy,
        Guid     dealerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama karyawan tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email karyawan tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(ktpNumber))
            throw new DomainException("Nomor KTP tidak boleh kosong.");
        if (salesLimit < 0)
            throw new DomainException("Sales limit tidak boleh negatif.");

        var karyawan = new Karyawan();
        karyawan.Raise(new KaryawanCreated(
            KaryawanId.New(),
            dealerId,
            name.Trim(),
            email.Trim(),
            ktpNumber.Trim(),
            gender,
            hireDate,
            phone?.Trim() ?? string.Empty,
            phoneAlt?.Trim(),
            address?.Trim() ?? string.Empty,
            salesLimit,
            jabatanId,
            createdBy,
            DateTime.UtcNow));
        return karyawan;
    }

    public static Karyawan Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var karyawan = new Karyawan();
        karyawan.Load(events);
        return karyawan;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(
        string  name,
        string  email,
        string  phone,
        string? phoneAlt,
        string  address,
        Guid    jabatanId,
        string  updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama karyawan tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email karyawan tidak boleh kosong.");

        var trimName     = name.Trim();
        var trimEmail    = email.Trim();
        var trimPhone    = phone?.Trim() ?? string.Empty;
        var trimPhoneAlt = phoneAlt?.Trim();
        var trimAddress  = address?.Trim() ?? string.Empty;

        if (Name == trimName && Email == trimEmail &&
            Phone == trimPhone && PhoneAlt == trimPhoneAlt &&
            Address == trimAddress && JabatanId == jabatanId)
            return;

        Raise(new KaryawanUpdated(Id, trimName, trimEmail, trimPhone, trimPhoneAlt, trimAddress, jabatanId, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string activatedBy)
    {
        if (IsActive)
            throw new DomainException("Karyawan sudah aktif.");
        Raise(new KaryawanActivated(Id, activatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string deactivatedBy)
    {
        if (!IsActive)
            throw new DomainException("Karyawan sudah tidak aktif.");
        Raise(new KaryawanDeactivated(Id, deactivatedBy, DateTime.UtcNow));
    }

    public void RecordResign(DateOnly resignDate, string recordedBy)
    {
        if (ResignDate.HasValue)
            throw new DomainException("Karyawan sudah tercatat mengundurkan diri.");
        Raise(new KaryawanResigned(Id, resignDate, recordedBy, DateTime.UtcNow));
    }

    public void SetLimit(decimal salesLimit, string setBy)
    {
        if (salesLimit < 0)
            throw new DomainException("Sales limit tidak boleh negatif.");
        Raise(new KaryawanLimitSet(Id, salesLimit, setBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case KaryawanCreated e:
                Id         = e.KaryawanId;
                DealerId   = e.DealerId;
                Name       = e.Name;
                Email      = e.Email;
                KtpNumber  = e.KtpNumber;
                Gender     = e.Gender;
                HireDate   = e.HireDate;
                Phone      = e.Phone;
                PhoneAlt   = e.PhoneAlt;
                Address    = e.Address;
                SalesLimit = e.SalesLimit;
                JabatanId  = e.JabatanId;
                IsActive   = true;
                break;

            case KaryawanUpdated e:
                Name      = e.Name;
                Email     = e.Email;
                Phone     = e.Phone;
                PhoneAlt  = e.PhoneAlt;
                Address   = e.Address;
                JabatanId = e.JabatanId;
                break;

            case KaryawanActivated:
                IsActive = true;
                break;

            case KaryawanDeactivated:
                IsActive = false;
                break;

            case KaryawanResigned e:
                ResignDate = e.ResignDate;
                break;

            case KaryawanLimitSet e:
                SalesLimit = e.SalesLimit;
                break;
        }
    }
}
