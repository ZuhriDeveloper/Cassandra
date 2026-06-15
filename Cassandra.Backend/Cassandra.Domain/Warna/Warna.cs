using Cassandra.Domain.Common;
using Cassandra.Domain.Warna.Events;

namespace Cassandra.Domain.Warna;

public class Warna : AggregateRoot<WarnaId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Warna() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Warna Create(string code, string name, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode warna tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama warna tidak boleh kosong.");

        var warna = new Warna();
        warna.Raise(new WarnaCreated(
            WarnaId.New(),
            dealerId,
            code.Trim().ToUpper(),
            name.Trim(),
            createdBy,
            DateTime.UtcNow));
        return warna;
    }

    public static Warna Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var warna = new Warna();
        warna.Load(events);
        return warna;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama warna tidak boleh kosong.");

        var trimmedName = name.Trim();
        if (Name == trimmedName) return;

        Raise(new WarnaUpdated(Id, trimmedName, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Warna sudah aktif.");

        Raise(new WarnaActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Warna sudah tidak aktif.");

        Raise(new WarnaDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case WarnaCreated e:
                Id = e.WarnaId;
                DealerId = e.DealerId;
                Code = e.Code;
                Name = e.Name;
                IsActive = true;
                break;

            case WarnaUpdated e:
                Name = e.Name;
                break;

            case WarnaActivated:
                IsActive = true;
                break;

            case WarnaDeactivated:
                IsActive = false;
                break;
        }
    }
}
