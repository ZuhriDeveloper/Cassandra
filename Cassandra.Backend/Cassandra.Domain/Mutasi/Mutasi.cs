using Cassandra.Domain.Common;
using Cassandra.Domain.Mutasi.Events;

namespace Cassandra.Domain.Mutasi;

public class Mutasi : AggregateRoot<MutasiId>
{
    public Guid DealerId { get; private set; }
    public string MutasiNumber { get; private set; } = default!;
    public DateOnly MutasiDate { get; private set; }
    public Guid SourceKiosId { get; private set; }
    public Guid DestinationKiosId { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyList<string> EngineNumbers { get; private set; } = [];
    public IReadOnlyList<MutasiKelengkapanValue> KelengkapanItems { get; private set; } = [];

    private Mutasi() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Mutasi Create(
        string mutasiNumber,
        DateOnly mutasiDate,
        Guid sourceKiosId,
        Guid destinationKiosId,
        IReadOnlyList<string> engineNumbers,
        IReadOnlyList<MutasiKelengkapanValue> kelengkapanItems,
        string createdBy,
        Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(mutasiNumber))
            throw new DomainException("Nomor mutasi tidak boleh kosong.");
        if (sourceKiosId == Guid.Empty)
            throw new DomainException("Kios asal tidak valid.");
        if (destinationKiosId == Guid.Empty)
            throw new DomainException("Kios tujuan tidak valid.");
        if (sourceKiosId == destinationKiosId)
            throw new DomainException("Kios asal dan kios tujuan tidak boleh sama.");
        if (engineNumbers == null || engineNumbers.Count == 0)
            throw new DomainException("Mutasi harus memiliki minimal satu nomor mesin.");

        var mutasi = new Mutasi();
        mutasi.Raise(new MutasiCreated(
            MutasiId.New(),
            dealerId,
            mutasiNumber.Trim().ToUpper(),
            mutasiDate,
            sourceKiosId,
            destinationKiosId,
            engineNumbers,
            kelengkapanItems ?? [],
            createdBy,
            DateTime.UtcNow));
        return mutasi;
    }

    public static Mutasi Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var mutasi = new Mutasi();
        mutasi.Load(events);
        return mutasi;
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        if (domainEvent is MutasiCreated e)
        {
            Id = e.Id;
            DealerId = e.DealerId;
            MutasiNumber = e.MutasiNumber;
            MutasiDate = e.MutasiDate;
            SourceKiosId = e.SourceKiosId;
            DestinationKiosId = e.DestinationKiosId;
            EngineNumbers = e.EngineNumbers;
            KelengkapanItems = e.KelengkapanItems;
            IsActive = true;
        }
    }
}
