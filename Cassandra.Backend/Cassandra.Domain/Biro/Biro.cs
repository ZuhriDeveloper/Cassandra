using Cassandra.Domain.Biro.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Domain.Biro;

public class Biro : AggregateRoot<BiroId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? Fax { get; private set; }
    public string? Pic { get; private set; }
    public string? Address { get; private set; }
    public decimal PphRate { get; private set; }
    public bool IsActive { get; private set; }

    private Biro() { }

    public static Biro Create(
        string code,
        string name,
        string? phone,
        string? fax,
        string? pic,
        string? address,
        decimal pphRate,
        string createdBy,
        Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode biro tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama biro tidak boleh kosong.");
        if (pphRate < 0)
            throw new DomainException("PPh rate tidak boleh negatif.");

        var biro = new Biro();
        biro.Raise(new BiroCreated(
            BiroId.New(),
            dealerId,
            code.Trim().ToUpper(),
            name.Trim(),
            phone?.Trim(),
            fax?.Trim(),
            pic?.Trim(),
            address?.Trim(),
            pphRate,
            createdBy,
            DateTime.UtcNow));
        return biro;
    }

    public static Biro Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var biro = new Biro();
        biro.Load(events);
        return biro;
    }

    public void Update(
        string name,
        string? phone,
        string? fax,
        string? pic,
        string? address,
        decimal pphRate,
        string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama biro tidak boleh kosong.");
        if (pphRate < 0)
            throw new DomainException("PPh rate tidak boleh negatif.");

        var trimName = name.Trim();
        var trimPhone = phone?.Trim();
        var trimFax = fax?.Trim();
        var trimPic = pic?.Trim();
        var trimAddress = address?.Trim();

        if (Name == trimName && Phone == trimPhone && Fax == trimFax &&
            Pic == trimPic && Address == trimAddress && PphRate == pphRate)
            return;

        Raise(new BiroUpdated(Id, trimName, trimPhone, trimFax, trimPic, trimAddress, pphRate, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Biro sudah aktif.");

        Raise(new BiroActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Biro sudah tidak aktif.");

        Raise(new BiroDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case BiroCreated e:
                Id = e.BiroId;
                DealerId = e.DealerId;
                Code = e.Code;
                Name = e.Name;
                Phone = e.Phone;
                Fax = e.Fax;
                Pic = e.Pic;
                Address = e.Address;
                PphRate = e.PphRate;
                IsActive = true;
                break;

            case BiroUpdated e:
                Name = e.Name;
                Phone = e.Phone;
                Fax = e.Fax;
                Pic = e.Pic;
                Address = e.Address;
                PphRate = e.PphRate;
                break;

            case BiroActivated:
                IsActive = true;
                break;

            case BiroDeactivated:
                IsActive = false;
                break;
        }
    }
}
