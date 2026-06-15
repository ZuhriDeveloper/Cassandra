using Cassandra.Domain.Common;
using Cassandra.Domain.Kelengkapan.Events;

namespace Cassandra.Domain.Kelengkapan;

public class Kelengkapan : AggregateRoot<KelengkapanId>
{
    public Guid DealerId { get; private set; }
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Kelengkapan() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Kelengkapan Create(string name, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama kelengkapan tidak boleh kosong.");

        var kelengkapan = new Kelengkapan();
        kelengkapan.Raise(new KelengkapanCreated(
            KelengkapanId.New(),
            dealerId,
            name.Trim(),
            createdBy,
            DateTime.UtcNow));
        return kelengkapan;
    }

    public static Kelengkapan Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var kelengkapan = new Kelengkapan();
        kelengkapan.Load(events);
        return kelengkapan;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama kelengkapan tidak boleh kosong.");

        var trimmedName = name.Trim();
        if (Name == trimmedName) return;

        Raise(new KelengkapanUpdated(Id, trimmedName, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Kelengkapan sudah aktif.");

        Raise(new KelengkapanActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Kelengkapan sudah tidak aktif.");

        Raise(new KelengkapanDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case KelengkapanCreated e:
                Id = e.KelengkapanId;
                DealerId = e.DealerId;
                Name = e.Name;
                IsActive = true;
                break;

            case KelengkapanUpdated e:
                Name = e.Name;
                break;

            case KelengkapanActivated:
                IsActive = true;
                break;

            case KelengkapanDeactivated:
                IsActive = false;
                break;
        }
    }
}
