using Cassandra.Domain.Common;
using Cassandra.Domain.PelanggaranWilayah.Events;

namespace Cassandra.Domain.PelanggaranWilayah;

public class PelanggaranWilayah : AggregateRoot<PelanggaranWilayahId>
{
    public Guid DealerId { get; private set; }
    public string AreaCode { get; private set; } = default!;
    public decimal ExtraFee { get; private set; }
    public bool IsActive { get; private set; }

    private PelanggaranWilayah() { }

    public static PelanggaranWilayah Create(string areaCode, decimal extraFee, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(areaCode))
            throw new DomainException("Kode area tidak boleh kosong.");
        if (extraFee < 0)
            throw new DomainException("Biaya tambahan tidak boleh negatif.");

        var pw = new PelanggaranWilayah();
        pw.Raise(new PelanggaranWilayahCreated(
            PelanggaranWilayahId.New(),
            dealerId,
            areaCode.Trim(),
            extraFee,
            createdBy,
            DateTime.UtcNow));
        return pw;
    }

    public static PelanggaranWilayah Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var pw = new PelanggaranWilayah();
        pw.Load(events);
        return pw;
    }

    public void Update(decimal extraFee, string updatedBy)
    {
        if (extraFee < 0)
            throw new DomainException("Biaya tambahan tidak boleh negatif.");

        if (ExtraFee == extraFee) return;

        Raise(new PelanggaranWilayahUpdated(Id, extraFee, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Pelanggaran wilayah sudah aktif.");

        Raise(new PelanggaranWilayahActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Pelanggaran wilayah sudah tidak aktif.");

        Raise(new PelanggaranWilayahDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case PelanggaranWilayahCreated e:
                Id = e.PelanggaranWilayahId;
                DealerId = e.DealerId;
                AreaCode = e.AreaCode;
                ExtraFee = e.ExtraFee;
                IsActive = true;
                break;

            case PelanggaranWilayahUpdated e:
                ExtraFee = e.ExtraFee;
                break;

            case PelanggaranWilayahActivated:
                IsActive = true;
                break;

            case PelanggaranWilayahDeactivated:
                IsActive = false;
                break;
        }
    }
}
