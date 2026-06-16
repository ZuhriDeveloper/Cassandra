using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTenor.Events;

namespace Cassandra.Domain.GrupTenor;

public class GrupTenor : AggregateRoot<GrupTenorId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private GrupTenor() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static GrupTenor Create(string code, string name, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode grup tenor tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama grup tenor tidak boleh kosong.");

        var gt = new GrupTenor();
        gt.Raise(new GrupTenorCreated(
            GrupTenorId.New(),
            dealerId,
            code.Trim().ToUpper(),
            name.Trim(),
            createdBy,
            DateTime.UtcNow));
        return gt;
    }

    public static GrupTenor Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var gt = new GrupTenor();
        gt.Load(events);
        return gt;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama grup tenor tidak boleh kosong.");

        var trimmed = name.Trim();
        if (Name == trimmed) return;

        Raise(new GrupTenorUpdated(Id, trimmed, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Grup tenor sudah aktif.");

        Raise(new GrupTenorActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Grup tenor sudah tidak aktif.");

        Raise(new GrupTenorDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case GrupTenorCreated e:
                Id = e.GrupTenorId;
                DealerId = e.DealerId;
                Code = e.Code;
                Name = e.Name;
                IsActive = true;
                break;

            case GrupTenorUpdated e:
                Name = e.Name;
                break;

            case GrupTenorActivated:
                IsActive = true;
                break;

            case GrupTenorDeactivated:
                IsActive = false;
                break;
        }
    }
}
