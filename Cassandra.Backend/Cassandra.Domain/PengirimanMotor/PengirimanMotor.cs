using Cassandra.Domain.Common;
using Cassandra.Domain.PengirimanMotor.Events;

namespace Cassandra.Domain.PengirimanMotor;

public class PengirimanMotor : AggregateRoot<PengirimanMotorId>
{
    public Guid     DealerId               { get; private set; }
    public Guid     RegistrasiPenjualanId  { get; private set; }
    public string   NoMesin                { get; private set; } = default!;
    public Guid     Driver1Id              { get; private set; }
    public Guid?    Driver2Id              { get; private set; }
    public DateOnly DeliveryDate           { get; private set; }
    public string?  Zona                   { get; private set; }

    private PengirimanMotor() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static PengirimanMotor Create(
        Guid     registrasiPenjualanId,
        string   noMesin,
        Guid     driver1Id,
        Guid?    driver2Id,
        DateOnly deliveryDate,
        string?  zona,
        string   createdBy,
        Guid     dealerId)
    {
        if (noMesin == null || string.IsNullOrWhiteSpace(noMesin))
            throw new DomainException("Nomor mesin tidak boleh kosong.");
        if (driver1Id == Guid.Empty)
            throw new DomainException("Driver 1 harus dipilih.");

        var pm = new PengirimanMotor();
        pm.Raise(new PengirimanMotorCreated(
            PengirimanMotorId.New(),
            dealerId,
            registrasiPenjualanId,
            noMesin.Trim(),
            driver1Id,
            driver2Id,
            deliveryDate,
            zona?.Trim(),
            createdBy,
            DateTime.UtcNow));
        return pm;
    }

    public static PengirimanMotor Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var pm = new PengirimanMotor();
        pm.Load(events);
        return pm;
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case PengirimanMotorCreated e:
                Id                    = e.Id;
                DealerId              = e.DealerId;
                RegistrasiPenjualanId = e.RegistrasiPenjualanId;
                NoMesin               = e.NoMesin;
                Driver1Id             = e.Driver1Id;
                Driver2Id             = e.Driver2Id;
                DeliveryDate          = e.DeliveryDate;
                Zona                  = e.Zona;
                break;
        }
    }
}
