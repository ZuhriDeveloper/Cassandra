using Cassandra.Domain.Common;
using Cassandra.Domain.SoPenerimaan.Events;

namespace Cassandra.Domain.SoPenerimaan;

public class SoPenerimaan : AggregateRoot<SoPenerimaanId>
{
    public Guid DealerId { get; private set; }
    public string SuratJalanId { get; private set; } = default!;
    public DateOnly SuratJalanDate { get; private set; }
    public Guid SoId { get; private set; }
    public IReadOnlyList<SoPenerimaanItemMotorValue> MotorItems { get; private set; } = [];
    public IReadOnlyList<SoPenerimaanItemKelengkapanValue> KelengkapanItems { get; private set; } = [];

    private SoPenerimaan() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static SoPenerimaan Create(
        string suratJalanId,
        DateOnly suratJalanDate,
        Guid soId,
        IReadOnlyList<SoPenerimaanItemMotorValue> motorItems,
        IReadOnlyList<SoPenerimaanItemKelengkapanValue> kelengkapanItems,
        string createdBy,
        Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(suratJalanId))
            throw new DomainException("Nomor surat jalan tidak boleh kosong.");
        if (motorItems == null || motorItems.Count == 0)
            throw new DomainException("SO Penerimaan harus memiliki minimal satu item motor.");

        var sp = new SoPenerimaan();
        sp.Raise(new SoPenerimaanCreated(
            SoPenerimaanId.New(),
            dealerId,
            suratJalanId.Trim(),
            suratJalanDate,
            soId,
            motorItems,
            kelengkapanItems ?? [],
            createdBy,
            DateTime.UtcNow));
        return sp;
    }

    public static SoPenerimaan Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var sp = new SoPenerimaan();
        sp.Load(events);
        return sp;
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        if (domainEvent is SoPenerimaanCreated e)
        {
            Id = e.Id;
            DealerId = e.DealerId;
            SuratJalanId = e.SuratJalanId;
            SuratJalanDate = e.SuratJalanDate;
            SoId = e.SoId;
            MotorItems = e.MotorItems;
            KelengkapanItems = e.KelengkapanItems;
        }
    }
}
