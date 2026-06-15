using Cassandra.Domain.Common;
using Cassandra.Domain.Mediator.Events;

namespace Cassandra.Domain.Mediator;

public class Mediator : AggregateRoot<MediatorId>
{
    public Guid    DealerId   { get; private set; }
    public string  Name       { get; private set; } = default!;
    public Guid    KaryawanId { get; private set; }
    public string  Address    { get; private set; } = default!;
    public decimal Limit      { get; private set; }
    public bool    IsActive   { get; private set; }

    private Mediator() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Mediator Create(
        string  name,
        Guid    karyawanId,
        string  address,
        decimal limit,
        string  createdBy,
        Guid    dealerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama mediator tidak boleh kosong.");
        if (karyawanId == Guid.Empty)
            throw new DomainException("Karyawan harus dipilih.");
        if (limit < 0)
            throw new DomainException("Limit mediator tidak boleh negatif.");

        var mediator = new Mediator();
        mediator.Raise(new MediatorCreated(
            MediatorId.New(),
            dealerId,
            name.Trim(),
            karyawanId,
            address?.Trim() ?? string.Empty,
            limit,
            createdBy,
            DateTime.UtcNow));
        return mediator;
    }

    public static Mediator Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var mediator = new Mediator();
        mediator.Load(events);
        return mediator;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, Guid karyawanId, string address, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama mediator tidak boleh kosong.");
        if (karyawanId == Guid.Empty)
            throw new DomainException("Karyawan harus dipilih.");

        var trimName    = name.Trim();
        var trimAddress = address?.Trim() ?? string.Empty;

        if (Name == trimName && KaryawanId == karyawanId && Address == trimAddress)
            return;

        Raise(new MediatorUpdated(Id, trimName, karyawanId, trimAddress, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string activatedBy)
    {
        if (IsActive)
            throw new DomainException("Mediator sudah aktif.");
        Raise(new MediatorActivated(Id, activatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string deactivatedBy)
    {
        if (!IsActive)
            throw new DomainException("Mediator sudah tidak aktif.");
        Raise(new MediatorDeactivated(Id, deactivatedBy, DateTime.UtcNow));
    }

    public void SetLimit(decimal limit, string setBy)
    {
        if (limit < 0)
            throw new DomainException("Limit mediator tidak boleh negatif.");
        Raise(new MediatorLimitSet(Id, limit, setBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case MediatorCreated e:
                Id         = e.MediatorId;
                DealerId   = e.DealerId;
                Name       = e.Name;
                KaryawanId = e.KaryawanId;
                Address    = e.Address;
                Limit      = e.Limit;
                IsActive   = true;
                break;

            case MediatorUpdated e:
                Name       = e.Name;
                KaryawanId = e.KaryawanId;
                Address    = e.Address;
                break;

            case MediatorActivated:
                IsActive = true;
                break;

            case MediatorDeactivated:
                IsActive = false;
                break;

            case MediatorLimitSet e:
                Limit = e.Limit;
                break;
        }
    }
}
