using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTipeMotor.Events;

namespace Cassandra.Domain.GrupTipeMotor;

public class GrupTipeMotor : AggregateRoot<GrupTipeMotorId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private GrupTipeMotor() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static GrupTipeMotor Create(string code, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode grup tipe motor tidak boleh kosong.");

        var grup = new GrupTipeMotor();
        grup.Raise(new GrupTipeMotorCreated(
            GrupTipeMotorId.New(),
            dealerId,
            code.Trim().ToUpper(),
            createdBy,
            DateTime.UtcNow));
        return grup;
    }

    public static GrupTipeMotor Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var grup = new GrupTipeMotor();
        grup.Load(events);
        return grup;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Grup tipe motor sudah aktif.");

        Raise(new GrupTipeMotorActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Grup tipe motor sudah tidak aktif.");

        Raise(new GrupTipeMotorDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case GrupTipeMotorCreated e:
                Id = e.GrupTipeMotorId;
                DealerId = e.DealerId;
                Code = e.Code;
                IsActive = true;
                break;

            case GrupTipeMotorActivated:
                IsActive = true;
                break;

            case GrupTipeMotorDeactivated:
                IsActive = false;
                break;
        }
    }
}
