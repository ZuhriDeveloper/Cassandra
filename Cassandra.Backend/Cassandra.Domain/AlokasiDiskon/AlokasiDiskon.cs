using Cassandra.Domain.Common;
using Cassandra.Domain.AlokasiDiskon.Events;

namespace Cassandra.Domain.AlokasiDiskon;

public class AlokasiDiskon : AggregateRoot<AlokasiDiskonId>
{
    public Guid DealerId { get; private set; }
    public Guid KaryawanId { get; private set; }
    public string DiscountLevel { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private AlokasiDiskon() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static AlokasiDiskon Create(Guid karyawanId, string discountLevel, string createdBy, Guid dealerId)
    {
        if (karyawanId == Guid.Empty)
            throw new DomainException("Karyawan harus dipilih.");
        if (string.IsNullOrWhiteSpace(discountLevel))
            throw new DomainException("Level discount tidak boleh kosong.");

        var ad = new AlokasiDiskon();
        ad.Raise(new AlokasiDiskonCreated(
            AlokasiDiskonId.New(),
            dealerId,
            karyawanId,
            discountLevel.Trim(),
            createdBy,
            DateTime.UtcNow));
        return ad;
    }

    public static AlokasiDiskon Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var ad = new AlokasiDiskon();
        ad.Load(events);
        return ad;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string discountLevel, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(discountLevel))
            throw new DomainException("Level discount tidak boleh kosong.");

        var trimmed = discountLevel.Trim();
        if (DiscountLevel == trimmed) return;

        Raise(new AlokasiDiskonUpdated(Id, trimmed, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Alokasi diskon sudah aktif.");

        Raise(new AlokasiDiskonActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Alokasi diskon sudah tidak aktif.");

        Raise(new AlokasiDiskonDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case AlokasiDiskonCreated e:
                Id = e.AlokasiDiskonId;
                DealerId = e.DealerId;
                KaryawanId = e.KaryawanId;
                DiscountLevel = e.DiscountLevel;
                IsActive = true;
                break;

            case AlokasiDiskonUpdated e:
                DiscountLevel = e.DiscountLevel;
                break;

            case AlokasiDiskonActivated:
                IsActive = true;
                break;

            case AlokasiDiskonDeactivated:
                IsActive = false;
                break;
        }
    }
}
