using Cassandra.Domain.AlokasiDiskon.Events;
using Cassandra.Domain.PengirimanMotor.Events;
using Cassandra.Domain.RegistrasiPenjualan.Events;
using Cassandra.Domain.BiayaBiroJasa.Events;
using Cassandra.Domain.Biro.Events;
using Cassandra.Domain.CabangLeasing.Events;
using Cassandra.Domain.Common;
using Cassandra.Domain.DaftarHargaLeasing.Events;
using Cassandra.Domain.Dealers.Events;
using Cassandra.Domain.Df.Events;
using Cassandra.Domain.Discount.Events;
using Cassandra.Domain.DiscountCash.Events;
using Cassandra.Domain.ExpenseType.Events;
using Cassandra.Domain.GlobalLeasing.Events;
using Cassandra.Domain.GrupTenor.Events;
using Cassandra.Domain.GrupTipeMotor.Events;
using Cassandra.Domain.Jabatan.Events;
using Cassandra.Domain.Karyawan.Events;
using Cassandra.Domain.Kelengkapan.Events;
using Cassandra.Domain.Kios.Events;
using Cassandra.Domain.Ledger.Events;
using Cassandra.Domain.Mediator.Events;
using Cassandra.Domain.MetodeKeuangan.Events;
using Cassandra.Domain.Mutasi.Events;
using Cassandra.Domain.PelanggaranWilayah.Events;
using Cassandra.Domain.Samsat.Events;
using Cassandra.Domain.So.Events;
using Cassandra.Domain.SoPenerimaan.Events;
using Cassandra.Domain.SoRetur.Events;
using Cassandra.Domain.Stock.Events;
using Cassandra.Domain.Tenor.Events;
using Cassandra.Domain.TipeMotor.Events;
using Cassandra.Domain.Warna.Events;

namespace Cassandra.Infrastructure.Persistence.EventStore;

public static class EventTypeRegistry
{
    private static readonly Dictionary<string, Type> Map = new()
    {
        // Dealer (platform / multi-tenant) events
        [nameof(DealerRegistered)]  = typeof(DealerRegistered),
        [nameof(DealerRenamed)]     = typeof(DealerRenamed),
        [nameof(DealerActivated)]   = typeof(DealerActivated),
        [nameof(DealerDeactivated)] = typeof(DealerDeactivated),

        // Jabatan (dealer-scoped) events
        [nameof(JabatanCreated)]    = typeof(JabatanCreated),
        [nameof(JabatanUpdated)]    = typeof(JabatanUpdated),
        [nameof(JabatanActivated)]  = typeof(JabatanActivated),
        [nameof(JabatanDeactivated)] = typeof(JabatanDeactivated),

        // Karyawan (dealer-scoped) events
        [nameof(KaryawanCreated)]     = typeof(KaryawanCreated),
        [nameof(KaryawanUpdated)]     = typeof(KaryawanUpdated),
        [nameof(KaryawanActivated)]   = typeof(KaryawanActivated),
        [nameof(KaryawanDeactivated)] = typeof(KaryawanDeactivated),
        [nameof(KaryawanResigned)]    = typeof(KaryawanResigned),
        [nameof(KaryawanLimitSet)]    = typeof(KaryawanLimitSet),

        // Kios (dealer-scoped) events
        [nameof(KiosCreated)]         = typeof(KiosCreated),
        [nameof(KiosUpdated)]         = typeof(KiosUpdated),
        [nameof(KiosActivated)]       = typeof(KiosActivated),
        [nameof(KiosDeactivated)]     = typeof(KiosDeactivated),
        [nameof(KiosLimitSet)]        = typeof(KiosLimitSet),

        // Mediator (dealer-scoped) events
        [nameof(MediatorCreated)]     = typeof(MediatorCreated),
        [nameof(MediatorUpdated)]     = typeof(MediatorUpdated),
        [nameof(MediatorActivated)]   = typeof(MediatorActivated),
        [nameof(MediatorDeactivated)] = typeof(MediatorDeactivated),
        [nameof(MediatorLimitSet)]    = typeof(MediatorLimitSet),

        // Warna (dealer-scoped) events
        [nameof(WarnaCreated)]     = typeof(WarnaCreated),
        [nameof(WarnaUpdated)]     = typeof(WarnaUpdated),
        [nameof(WarnaActivated)]   = typeof(WarnaActivated),
        [nameof(WarnaDeactivated)] = typeof(WarnaDeactivated),

        // GrupTipeMotor (dealer-scoped) events
        [nameof(GrupTipeMotorCreated)]     = typeof(GrupTipeMotorCreated),
        [nameof(GrupTipeMotorActivated)]   = typeof(GrupTipeMotorActivated),
        [nameof(GrupTipeMotorDeactivated)] = typeof(GrupTipeMotorDeactivated),

        // TipeMotor (dealer-scoped) events
        [nameof(TipeMotorCreated)]     = typeof(TipeMotorCreated),
        [nameof(TipeMotorUpdated)]     = typeof(TipeMotorUpdated),
        [nameof(TipeMotorActivated)]   = typeof(TipeMotorActivated),
        [nameof(TipeMotorDeactivated)] = typeof(TipeMotorDeactivated),
        [nameof(TipeMotorColorsSet)]   = typeof(TipeMotorColorsSet),

        // Kelengkapan (dealer-scoped) events
        [nameof(KelengkapanCreated)]     = typeof(KelengkapanCreated),
        [nameof(KelengkapanUpdated)]     = typeof(KelengkapanUpdated),
        [nameof(KelengkapanActivated)]   = typeof(KelengkapanActivated),
        [nameof(KelengkapanDeactivated)] = typeof(KelengkapanDeactivated),

        // MetodeKeuangan (dealer-scoped) events
        [nameof(MetodeKeuanganCreated)]     = typeof(MetodeKeuanganCreated),
        [nameof(MetodeKeuanganUpdated)]     = typeof(MetodeKeuanganUpdated),
        [nameof(MetodeKeuanganActivated)]   = typeof(MetodeKeuanganActivated),
        [nameof(MetodeKeuanganDeactivated)] = typeof(MetodeKeuanganDeactivated),

        // GlobalLeasing (dealer-scoped) events
        [nameof(GlobalLeasingCreated)]     = typeof(GlobalLeasingCreated),
        [nameof(GlobalLeasingUpdated)]     = typeof(GlobalLeasingUpdated),
        [nameof(GlobalLeasingActivated)]   = typeof(GlobalLeasingActivated),
        [nameof(GlobalLeasingDeactivated)] = typeof(GlobalLeasingDeactivated),

        // CabangLeasing (dealer-scoped) events
        [nameof(CabangLeasingCreated)]     = typeof(CabangLeasingCreated),
        [nameof(CabangLeasingUpdated)]     = typeof(CabangLeasingUpdated),
        [nameof(CabangLeasingActivated)]   = typeof(CabangLeasingActivated),
        [nameof(CabangLeasingDeactivated)] = typeof(CabangLeasingDeactivated),

        // GrupTenor (dealer-scoped) events
        [nameof(GrupTenorCreated)]     = typeof(GrupTenorCreated),
        [nameof(GrupTenorUpdated)]     = typeof(GrupTenorUpdated),
        [nameof(GrupTenorActivated)]   = typeof(GrupTenorActivated),
        [nameof(GrupTenorDeactivated)] = typeof(GrupTenorDeactivated),

        // Tenor (dealer-scoped) events
        [nameof(TenorCreated)]     = typeof(TenorCreated),
        [nameof(TenorUpdated)]     = typeof(TenorUpdated),
        [nameof(TenorActivated)]   = typeof(TenorActivated),
        [nameof(TenorDeactivated)] = typeof(TenorDeactivated),

        // Df (dealer-singleton) events
        [nameof(DfSet)] = typeof(DfSet),

        // DaftarHargaLeasing (dealer-scoped) events
        [nameof(DaftarHargaLeasingCreated)]      = typeof(DaftarHargaLeasingCreated),
        [nameof(DaftarHargaLeasingUpdated)]      = typeof(DaftarHargaLeasingUpdated),
        [nameof(DaftarHargaLeasingActivated)]    = typeof(DaftarHargaLeasingActivated),
        [nameof(DaftarHargaLeasingDeactivated)]  = typeof(DaftarHargaLeasingDeactivated),
        [nameof(DaftarHargaLeasingItemsSet)]     = typeof(DaftarHargaLeasingItemsSet),

        // Discount (dealer-scoped) events
        [nameof(DiscountCreated)]      = typeof(DiscountCreated),
        [nameof(DiscountUpdated)]      = typeof(DiscountUpdated),
        [nameof(DiscountActivated)]    = typeof(DiscountActivated),
        [nameof(DiscountDeactivated)]  = typeof(DiscountDeactivated),
        [nameof(DiscountItemsSet)]     = typeof(DiscountItemsSet),

        // DiscountCash (dealer-scoped) events
        [nameof(DiscountCashCreated)]     = typeof(DiscountCashCreated),
        [nameof(DiscountCashUpdated)]     = typeof(DiscountCashUpdated),
        [nameof(DiscountCashActivated)]   = typeof(DiscountCashActivated),
        [nameof(DiscountCashDeactivated)] = typeof(DiscountCashDeactivated),

        // AlokasiDiskon (dealer-scoped) events
        [nameof(AlokasiDiskonCreated)]     = typeof(AlokasiDiskonCreated),
        [nameof(AlokasiDiskonUpdated)]     = typeof(AlokasiDiskonUpdated),
        [nameof(AlokasiDiskonActivated)]   = typeof(AlokasiDiskonActivated),
        [nameof(AlokasiDiskonDeactivated)] = typeof(AlokasiDiskonDeactivated),

        // Phase 4: Service Bureau & Finance Config

        // Samsat (dealer-scoped) events
        [nameof(SamsatCreated)]     = typeof(SamsatCreated),
        [nameof(SamsatUpdated)]     = typeof(SamsatUpdated),
        [nameof(SamsatActivated)]   = typeof(SamsatActivated),
        [nameof(SamsatDeactivated)] = typeof(SamsatDeactivated),
        [nameof(SamsatCitiesSet)]   = typeof(SamsatCitiesSet),

        // Biro (dealer-scoped) events
        [nameof(BiroCreated)]     = typeof(BiroCreated),
        [nameof(BiroUpdated)]     = typeof(BiroUpdated),
        [nameof(BiroActivated)]   = typeof(BiroActivated),
        [nameof(BiroDeactivated)] = typeof(BiroDeactivated),

        // BiayaBiroJasa (dealer-scoped) events
        [nameof(BiayaBiroJasaCreated)]     = typeof(BiayaBiroJasaCreated),
        [nameof(BiayaBiroJasaActivated)]   = typeof(BiayaBiroJasaActivated),
        [nameof(BiayaBiroJasaDeactivated)] = typeof(BiayaBiroJasaDeactivated),
        [nameof(BiayaBiroJasaItemsSet)]    = typeof(BiayaBiroJasaItemsSet),

        // ExpenseType (dealer-scoped) events
        [nameof(ExpenseTypeCreated)]     = typeof(ExpenseTypeCreated),
        [nameof(ExpenseTypeUpdated)]     = typeof(ExpenseTypeUpdated),
        [nameof(ExpenseTypeActivated)]   = typeof(ExpenseTypeActivated),
        [nameof(ExpenseTypeDeactivated)] = typeof(ExpenseTypeDeactivated),

        // Ledger (dealer-scoped) events
        [nameof(LedgerCreated)]     = typeof(LedgerCreated),
        [nameof(LedgerUpdated)]     = typeof(LedgerUpdated),
        [nameof(LedgerActivated)]   = typeof(LedgerActivated),
        [nameof(LedgerDeactivated)] = typeof(LedgerDeactivated),

        // PelanggaranWilayah (dealer-scoped) events
        [nameof(PelanggaranWilayahCreated)]     = typeof(PelanggaranWilayahCreated),
        [nameof(PelanggaranWilayahUpdated)]     = typeof(PelanggaranWilayahUpdated),
        [nameof(PelanggaranWilayahActivated)]   = typeof(PelanggaranWilayahActivated),
        [nameof(PelanggaranWilayahDeactivated)] = typeof(PelanggaranWilayahDeactivated),

        // Phase 5: Inventory & Stock

        // So (dealer-scoped) events
        [nameof(SoCreated)]       = typeof(SoCreated),
        [nameof(SoStatusChanged)] = typeof(SoStatusChanged),
        [nameof(SoDeleted)]       = typeof(SoDeleted),

        // Stock (dealer-scoped) events
        [nameof(StockCreated)]       = typeof(StockCreated),
        [nameof(StockStatusChanged)] = typeof(StockStatusChanged),
        [nameof(StockMoved)]         = typeof(StockMoved),

        // SoPenerimaan (dealer-scoped) events
        [nameof(SoPenerimaanCreated)] = typeof(SoPenerimaanCreated),

        // SoRetur (dealer-scoped) events
        [nameof(SoReturCreated)] = typeof(SoReturCreated),

        // Mutasi (dealer-scoped) events
        [nameof(MutasiCreated)] = typeof(MutasiCreated),

        // Phase 6: Sales

        // RegistrasiPenjualan (dealer-scoped) events
        [nameof(RegistrasiPenjualanCreated)]         = typeof(RegistrasiPenjualanCreated),
        [nameof(RegistrasiPenjualanApproved)]        = typeof(RegistrasiPenjualanApproved),
        [nameof(RegistrasiPenjualanSent)]            = typeof(RegistrasiPenjualanSent),
        [nameof(RegistrasiPenjualanVoided)]          = typeof(RegistrasiPenjualanVoided),
        [nameof(RegistrasiPenjualanEnableToVoidSet)] = typeof(RegistrasiPenjualanEnableToVoidSet),

        // PengirimanMotor (dealer-scoped) events
        [nameof(PengirimanMotorCreated)] = typeof(PengirimanMotorCreated),
    };

    public static Type Resolve(string eventType) =>
        Map.TryGetValue(eventType, out var type)
            ? type
            : throw new InvalidOperationException($"Unknown event type: '{eventType}'.");

    public static string GetName(IDomainEvent evt) => evt.GetType().Name;
}
