using Cassandra.Domain.Common;
using Cassandra.Domain.Tenor.Events;

namespace Cassandra.Domain.Tenor;

public class Tenor : AggregateRoot<TenorId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public int Months { get; private set; }
    public Guid GrupTenorId { get; private set; }
    public bool IsActive { get; private set; }

    private Tenor() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Tenor Create(string code, string name, int months, Guid grupTenorId, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode tenor tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama tenor tidak boleh kosong.");
        if (months <= 0)
            throw new DomainException("Jumlah bulan tenor harus lebih dari 0.");
        if (grupTenorId == Guid.Empty)
            throw new DomainException("Grup tenor harus dipilih.");

        var tenor = new Tenor();
        tenor.Raise(new TenorCreated(
            TenorId.New(),
            dealerId,
            code.Trim().ToUpper(),
            name.Trim(),
            months,
            grupTenorId,
            createdBy,
            DateTime.UtcNow));
        return tenor;
    }

    public static Tenor Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var tenor = new Tenor();
        tenor.Load(events);
        return tenor;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, int months, Guid grupTenorId, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama tenor tidak boleh kosong.");
        if (months <= 0)
            throw new DomainException("Jumlah bulan tenor harus lebih dari 0.");
        if (grupTenorId == Guid.Empty)
            throw new DomainException("Grup tenor harus dipilih.");

        var trimName = name.Trim();
        if (Name == trimName && Months == months && GrupTenorId == grupTenorId) return;

        Raise(new TenorUpdated(Id, trimName, months, grupTenorId, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Tenor sudah aktif.");

        Raise(new TenorActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Tenor sudah tidak aktif.");

        Raise(new TenorDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case TenorCreated e:
                Id = e.TenorId;
                DealerId = e.DealerId;
                Code = e.Code;
                Name = e.Name;
                Months = e.Months;
                GrupTenorId = e.GrupTenorId;
                IsActive = true;
                break;

            case TenorUpdated e:
                Name = e.Name;
                Months = e.Months;
                GrupTenorId = e.GrupTenorId;
                break;

            case TenorActivated:
                IsActive = true;
                break;

            case TenorDeactivated:
                IsActive = false;
                break;
        }
    }
}
