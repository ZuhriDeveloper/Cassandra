using Cassandra.Domain.Common;
using Cassandra.Domain.MetodeKeuangan.Events;

namespace Cassandra.Domain.MetodeKeuangan;

public class MetodeKeuangan : AggregateRoot<MetodeKeuanganId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private MetodeKeuangan() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static MetodeKeuangan Create(string code, string name, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode metode keuangan tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama metode keuangan tidak boleh kosong.");

        var mk = new MetodeKeuangan();
        mk.Raise(new MetodeKeuanganCreated(
            MetodeKeuanganId.New(),
            dealerId,
            code.Trim().ToUpper(),
            name.Trim(),
            createdBy,
            DateTime.UtcNow));
        return mk;
    }

    public static MetodeKeuangan Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var mk = new MetodeKeuangan();
        mk.Load(events);
        return mk;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama metode keuangan tidak boleh kosong.");

        var trimmed = name.Trim();
        if (Name == trimmed) return;

        Raise(new MetodeKeuanganUpdated(Id, trimmed, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Metode keuangan sudah aktif.");

        Raise(new MetodeKeuanganActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Metode keuangan sudah tidak aktif.");

        Raise(new MetodeKeuanganDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case MetodeKeuanganCreated e:
                Id = e.MetodeKeuanganId;
                DealerId = e.DealerId;
                Code = e.Code;
                Name = e.Name;
                IsActive = true;
                break;

            case MetodeKeuanganUpdated e:
                Name = e.Name;
                break;

            case MetodeKeuanganActivated:
                IsActive = true;
                break;

            case MetodeKeuanganDeactivated:
                IsActive = false;
                break;
        }
    }
}
