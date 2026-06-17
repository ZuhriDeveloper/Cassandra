using Cassandra.Domain.Common;
using Cassandra.Domain.Stock.Events;

namespace Cassandra.Domain.Stock;

public class Stock : AggregateRoot<StockId>
{
    public Guid DealerId { get; private set; }
    public string NoMesin { get; private set; } = default!;
    public string NoRangka { get; private set; } = default!;
    public Guid TipeMotorId { get; private set; }
    public Guid WarnaId { get; private set; }
    public Guid KiosId { get; private set; }
    public string SuratJalanId { get; private set; } = default!;
    public DateOnly SuratJalanDate { get; private set; }
    public Guid SoId { get; private set; }
    public string AssemblyYear { get; private set; } = default!;
    public string Status { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Stock() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Stock Create(
        string noMesin,
        string noRangka,
        Guid tipeMotorId,
        Guid warnaId,
        Guid kiosId,
        string suratJalanId,
        DateOnly suratJalanDate,
        Guid soId,
        string assemblyYear,
        string createdBy,
        Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(noMesin))
            throw new DomainException("Nomor mesin tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(noRangka))
            throw new DomainException("Nomor rangka tidak boleh kosong.");

        var stock = new Stock();
        stock.Raise(new StockCreated(
            StockId.New(),
            dealerId,
            noMesin.Trim(),
            noRangka.Trim(),
            tipeMotorId,
            warnaId,
            kiosId,
            suratJalanId,
            suratJalanDate,
            soId,
            assemblyYear,
            createdBy,
            DateTime.UtcNow));
        return stock;
    }

    public static Stock Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var stock = new Stock();
        stock.Load(events);
        return stock;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void ChangeStatus(string status, string updatedBy)
    {
        Raise(new StockStatusChanged(Id, status, updatedBy, DateTime.UtcNow));
    }

    public void MoveToKios(Guid newKiosId, string updatedBy)
    {
        if (newKiosId == KiosId) return;
        Raise(new StockMoved(Id, newKiosId, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case StockCreated e:
                Id = e.Id;
                DealerId = e.DealerId;
                NoMesin = e.NoMesin;
                NoRangka = e.NoRangka;
                TipeMotorId = e.TipeMotorId;
                WarnaId = e.WarnaId;
                KiosId = e.KiosId;
                SuratJalanId = e.SuratJalanId;
                SuratJalanDate = e.SuratJalanDate;
                SoId = e.SoId;
                AssemblyYear = e.AssemblyYear;
                Status = StockStatus.TERSEDIA;
                IsActive = true;
                break;

            case StockStatusChanged e:
                Status = e.Status;
                break;

            case StockMoved e:
                KiosId = e.NewKiosId;
                break;
        }
    }
}
