using Cassandra.Domain.Common;
using Cassandra.Domain.TipeMotor.Events;

namespace Cassandra.Domain.TipeMotor;

public class TipeMotor : AggregateRoot<TipeMotorId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public Guid GrupTipeMotorId { get; private set; }
    public string ShortName { get; private set; } = default!;
    public string ProductCode { get; private set; } = default!;
    public string WmsCode { get; private set; } = default!;
    public string AhmCode { get; private set; } = default!;
    public string EngineNumberFormat { get; private set; } = default!;
    public string ChassisNumberFormat { get; private set; } = default!;
    public decimal NettPrice { get; private set; }
    public decimal OrJakarta { get; private set; }
    public decimal OrTangerang { get; private set; }
    public decimal BbnJakarta { get; private set; }
    public decimal BbnTangerang { get; private set; }
    public bool IsActive { get; private set; }

    private List<Guid> _warnaIds = new();
    public IReadOnlyList<Guid> WarnaIds => _warnaIds.AsReadOnly();

    private TipeMotor() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static TipeMotor Create(
        string code,
        Guid grupTipeMotorId,
        string shortName,
        string productCode,
        string wmsCode,
        string ahmCode,
        string engineNumberFormat,
        string chassisNumberFormat,
        decimal nettPrice,
        decimal orJakarta,
        decimal orTangerang,
        decimal bbnJakarta,
        decimal bbnTangerang,
        string createdBy,
        Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode tipe motor tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(shortName))
            throw new DomainException("Nama singkat tipe motor tidak boleh kosong.");
        if (grupTipeMotorId == Guid.Empty)
            throw new DomainException("Grup tipe motor harus dipilih.");

        var tipe = new TipeMotor();
        tipe.Raise(new TipeMotorCreated(
            TipeMotorId.New(),
            dealerId,
            code.Trim().ToUpper(),
            grupTipeMotorId,
            shortName.Trim(),
            productCode?.Trim() ?? string.Empty,
            wmsCode?.Trim() ?? string.Empty,
            ahmCode?.Trim() ?? string.Empty,
            engineNumberFormat?.Trim() ?? string.Empty,
            chassisNumberFormat?.Trim() ?? string.Empty,
            nettPrice,
            orJakarta,
            orTangerang,
            bbnJakarta,
            bbnTangerang,
            createdBy,
            DateTime.UtcNow));
        return tipe;
    }

    public static TipeMotor Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var tipe = new TipeMotor();
        tipe.Load(events);
        return tipe;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(
        Guid grupTipeMotorId,
        string shortName,
        string productCode,
        string wmsCode,
        string ahmCode,
        string engineNumberFormat,
        string chassisNumberFormat,
        decimal nettPrice,
        decimal orJakarta,
        decimal orTangerang,
        decimal bbnJakarta,
        decimal bbnTangerang,
        string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(shortName))
            throw new DomainException("Nama singkat tipe motor tidak boleh kosong.");
        if (grupTipeMotorId == Guid.Empty)
            throw new DomainException("Grup tipe motor harus dipilih.");

        var trimShortName = shortName.Trim();
        var trimProductCode = productCode?.Trim() ?? string.Empty;
        var trimWmsCode = wmsCode?.Trim() ?? string.Empty;
        var trimAhmCode = ahmCode?.Trim() ?? string.Empty;
        var trimEngineFormat = engineNumberFormat?.Trim() ?? string.Empty;
        var trimChassisFormat = chassisNumberFormat?.Trim() ?? string.Empty;

        if (GrupTipeMotorId == grupTipeMotorId &&
            ShortName == trimShortName &&
            ProductCode == trimProductCode &&
            WmsCode == trimWmsCode &&
            AhmCode == trimAhmCode &&
            EngineNumberFormat == trimEngineFormat &&
            ChassisNumberFormat == trimChassisFormat &&
            NettPrice == nettPrice &&
            OrJakarta == orJakarta &&
            OrTangerang == orTangerang &&
            BbnJakarta == bbnJakarta &&
            BbnTangerang == bbnTangerang)
            return;

        Raise(new TipeMotorUpdated(
            Id,
            grupTipeMotorId,
            trimShortName,
            trimProductCode,
            trimWmsCode,
            trimAhmCode,
            trimEngineFormat,
            trimChassisFormat,
            nettPrice,
            orJakarta,
            orTangerang,
            bbnJakarta,
            bbnTangerang,
            updatedBy,
            DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Tipe motor sudah aktif.");

        Raise(new TipeMotorActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Tipe motor sudah tidak aktif.");

        Raise(new TipeMotorDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void SetColors(IReadOnlyList<Guid> warnaIds, string updatedBy)
    {
        Raise(new TipeMotorColorsSet(Id, warnaIds, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case TipeMotorCreated e:
                Id = e.TipeMotorId;
                DealerId = e.DealerId;
                Code = e.Code;
                GrupTipeMotorId = e.GrupTipeMotorId;
                ShortName = e.ShortName;
                ProductCode = e.ProductCode;
                WmsCode = e.WmsCode;
                AhmCode = e.AhmCode;
                EngineNumberFormat = e.EngineNumberFormat;
                ChassisNumberFormat = e.ChassisNumberFormat;
                NettPrice = e.NettPrice;
                OrJakarta = e.OrJakarta;
                OrTangerang = e.OrTangerang;
                BbnJakarta = e.BbnJakarta;
                BbnTangerang = e.BbnTangerang;
                IsActive = true;
                break;

            case TipeMotorUpdated e:
                GrupTipeMotorId = e.GrupTipeMotorId;
                ShortName = e.ShortName;
                ProductCode = e.ProductCode;
                WmsCode = e.WmsCode;
                AhmCode = e.AhmCode;
                EngineNumberFormat = e.EngineNumberFormat;
                ChassisNumberFormat = e.ChassisNumberFormat;
                NettPrice = e.NettPrice;
                OrJakarta = e.OrJakarta;
                OrTangerang = e.OrTangerang;
                BbnJakarta = e.BbnJakarta;
                BbnTangerang = e.BbnTangerang;
                break;

            case TipeMotorActivated:
                IsActive = true;
                break;

            case TipeMotorDeactivated:
                IsActive = false;
                break;

            case TipeMotorColorsSet e:
                _warnaIds = [.. e.WarnaIds];
                break;
        }
    }
}
