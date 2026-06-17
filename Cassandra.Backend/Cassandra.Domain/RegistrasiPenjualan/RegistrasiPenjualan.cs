using Cassandra.Domain.Common;
using Cassandra.Domain.RegistrasiPenjualan.Events;

namespace Cassandra.Domain.RegistrasiPenjualan;

public class RegistrasiPenjualan : AggregateRoot<RegistrasiPenjualanId>
{
    public Guid     DealerId               { get; private set; }
    public string   NoPenjualan            { get; private set; } = default!;
    public DateOnly SaleDate               { get; private set; }
    public Guid     KaryawanId             { get; private set; }
    public Guid     KiosId                 { get; private set; }
    public Guid?    MediatorId             { get; private set; }
    public string   MetodePenjualan        { get; private set; } = default!;
    public string   TipePenjualan          { get; private set; } = default!;
    public string   NoMesin                { get; private set; } = default!;
    public string   NoRangka               { get; private set; } = default!;
    public string   NamaCustomer           { get; private set; } = default!;
    public string   Address                { get; private set; } = default!;
    public string   Phone                  { get; private set; } = default!;
    public string?  Phone1                 { get; private set; }
    public string?  Phone2                 { get; private set; }
    public decimal  OffRoad                { get; private set; }
    public decimal  Bbn                    { get; private set; }
    public decimal  Discount               { get; private set; }
    public decimal  ApprovedDiscount       { get; private set; }
    public decimal  OriginalDiscount       { get; private set; }
    public decimal  Total                  { get; private set; }
    public decimal  AmbilUang              { get; private set; }
    public decimal  Dp                     { get; private set; }
    public decimal  Angsuran               { get; private set; }
    public decimal  Tac                    { get; private set; }
    public Guid?    DaftarHargaLeasingId   { get; private set; }
    public string?  TenorCode              { get; private set; }
    public string   TipeMotorCode          { get; private set; } = default!;
    public string   WarnaName              { get; private set; } = default!;
    public string   SerahTerimaKendaraanId { get; private set; } = default!;
    public string?  TandaTerimaSementaraId { get; private set; }
    public string   Kelengkapan            { get; private set; } = default!;
    public bool     IsApproved             { get; private set; }
    public bool     IsSent                 { get; private set; }
    public bool     IsVoid                 { get; private set; }
    public bool     EnableToVoid           { get; private set; }

    private RegistrasiPenjualan() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static RegistrasiPenjualan Create(
        string   noPenjualan,
        DateOnly saleDate,
        Guid     karyawanId,
        Guid     kiosId,
        Guid?    mediatorId,
        string   metodePenjualan,
        string   tipePenjualan,
        string   noMesin,
        string   noRangka,
        string   namaCustomer,
        string   address,
        string   phone,
        string?  phone1,
        string?  phone2,
        decimal  offRoad,
        decimal  bbn,
        decimal  discount,
        decimal  approvedDiscount,
        decimal  originalDiscount,
        decimal  total,
        decimal  ambilUang,
        decimal  dp,
        decimal  angsuran,
        decimal  tac,
        Guid?    daftarHargaLeasingId,
        string?  tenorCode,
        string   tipeMotorCode,
        string   warnaName,
        string   serahTerimaKendaraanId,
        string?  tandaTerimaSementaraId,
        string   kelengkapan,
        bool     isApproved,
        string   createdBy,
        Guid     dealerId)
    {
        if (string.IsNullOrWhiteSpace(noPenjualan))
            throw new DomainException("Nomor penjualan tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(noMesin))
            throw new DomainException("Nomor mesin tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(namaCustomer))
            throw new DomainException("Nama customer tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Nomor telepon tidak boleh kosong.");
        if (total <= 0)
            throw new DomainException("Total harga harus lebih dari nol.");

        var reg = new RegistrasiPenjualan();
        reg.Raise(new RegistrasiPenjualanCreated(
            RegistrasiPenjualanId.New(),
            dealerId,
            noPenjualan.Trim().ToUpper(),
            saleDate,
            karyawanId,
            kiosId,
            mediatorId,
            metodePenjualan,
            tipePenjualan,
            noMesin.Trim(),
            noRangka.Trim(),
            namaCustomer.Trim(),
            address?.Trim() ?? string.Empty,
            phone.Trim(),
            phone1?.Trim(),
            phone2?.Trim(),
            offRoad,
            bbn,
            discount,
            approvedDiscount,
            originalDiscount,
            total,
            ambilUang,
            dp,
            angsuran,
            tac,
            daftarHargaLeasingId,
            tenorCode,
            tipeMotorCode,
            warnaName,
            serahTerimaKendaraanId.Trim(),
            tandaTerimaSementaraId?.Trim(),
            kelengkapan,
            isApproved,
            createdBy,
            DateTime.UtcNow));
        return reg;
    }

    public static RegistrasiPenjualan Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var reg = new RegistrasiPenjualan();
        reg.Load(events);
        return reg;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Approve(decimal approvedDiscount, string approvedBy)
    {
        if (IsVoid)
            throw new DomainException("Registrasi penjualan sudah di-void.");
        if (IsApproved)
            throw new DomainException("Registrasi penjualan sudah disetujui.");

        Raise(new RegistrasiPenjualanApproved(Id, approvedDiscount, approvedBy, DateTime.UtcNow));
    }

    public void MarkAsSent(string sentBy)
    {
        if (IsVoid)
            throw new DomainException("Registrasi penjualan sudah di-void.");
        if (!IsApproved)
            throw new DomainException("Registrasi penjualan belum disetujui.");
        if (IsSent)
            throw new DomainException("Registrasi penjualan sudah dikirim.");

        Raise(new RegistrasiPenjualanSent(Id, sentBy, DateTime.UtcNow));
    }

    public void Void(string voidedBy)
    {
        if (IsVoid)
            throw new DomainException("Registrasi penjualan sudah di-void.");
        if (!EnableToVoid)
            throw new DomainException("Registrasi penjualan tidak dapat di-void saat ini.");

        Raise(new RegistrasiPenjualanVoided(Id, voidedBy, DateTime.UtcNow));
    }

    public void SetEnableToVoid(bool enableToVoid, string updatedBy)
    {
        if (IsVoid)
            throw new DomainException("Registrasi penjualan sudah di-void.");

        Raise(new RegistrasiPenjualanEnableToVoidSet(Id, enableToVoid, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case RegistrasiPenjualanCreated e:
                Id                    = e.Id;
                DealerId              = e.DealerId;
                NoPenjualan           = e.NoPenjualan;
                SaleDate              = e.SaleDate;
                KaryawanId            = e.KaryawanId;
                KiosId                = e.KiosId;
                MediatorId            = e.MediatorId;
                MetodePenjualan       = e.MetodePenjualan;
                TipePenjualan         = e.TipePenjualan;
                NoMesin               = e.NoMesin;
                NoRangka              = e.NoRangka;
                NamaCustomer          = e.NamaCustomer;
                Address               = e.Address;
                Phone                 = e.Phone;
                Phone1                = e.Phone1;
                Phone2                = e.Phone2;
                OffRoad               = e.OffRoad;
                Bbn                   = e.Bbn;
                Discount              = e.Discount;
                ApprovedDiscount      = e.ApprovedDiscount;
                OriginalDiscount      = e.OriginalDiscount;
                Total                 = e.Total;
                AmbilUang             = e.AmbilUang;
                Dp                    = e.Dp;
                Angsuran              = e.Angsuran;
                Tac                   = e.Tac;
                DaftarHargaLeasingId  = e.DaftarHargaLeasingId;
                TenorCode             = e.TenorCode;
                TipeMotorCode         = e.TipeMotorCode;
                WarnaName             = e.WarnaName;
                SerahTerimaKendaraanId = e.SerahTerimaKendaraanId;
                TandaTerimaSementaraId = e.TandaTerimaSementaraId;
                Kelengkapan           = e.Kelengkapan;
                IsApproved            = e.IsApproved;
                IsSent                = false;
                IsVoid                = false;
                EnableToVoid          = false;
                break;

            case RegistrasiPenjualanApproved e:
                ApprovedDiscount = e.ApprovedDiscount;
                IsApproved       = true;
                break;

            case RegistrasiPenjualanSent:
                IsSent = true;
                break;

            case RegistrasiPenjualanVoided:
                IsVoid = true;
                break;

            case RegistrasiPenjualanEnableToVoidSet e:
                EnableToVoid = e.EnableToVoid;
                break;
        }
    }
}
