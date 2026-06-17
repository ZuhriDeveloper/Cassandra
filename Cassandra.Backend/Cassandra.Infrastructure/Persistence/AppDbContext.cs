using Cassandra.Application.Contracts.Dealers;
using Cassandra.Infrastructure.Identity;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, ICurrentDealer currentDealer)
    : IdentityDbContext<ApplicationUser>(options)
{
    private Guid? CurrentDealerId => currentDealer.DealerIdOrNull;

    public DbSet<StoredEvent> StoredEvents => Set<StoredEvent>();
    public DbSet<DealerReadModel> DealerReadModels => Set<DealerReadModel>();
    public DbSet<JabatanReadModel> JabatanReadModels => Set<JabatanReadModel>();
    public DbSet<KaryawanReadModel> KaryawanReadModels => Set<KaryawanReadModel>();
    public DbSet<KiosReadModel> KiosReadModels => Set<KiosReadModel>();
    public DbSet<MediatorReadModel> MediatorReadModels => Set<MediatorReadModel>();
    public DbSet<WarnaReadModel> WarnaReadModels => Set<WarnaReadModel>();
    public DbSet<GrupTipeMotorReadModel> GrupTipeMotorReadModels => Set<GrupTipeMotorReadModel>();
    public DbSet<TipeMotorReadModel> TipeMotorReadModels => Set<TipeMotorReadModel>();
    public DbSet<TipeMotorWarnaReadModel> TipeMotorWarnaReadModels => Set<TipeMotorWarnaReadModel>();
    public DbSet<KelengkapanReadModel> KelengkapanReadModels => Set<KelengkapanReadModel>();

    // Phase 3: Leasing & Financing master data
    public DbSet<MetodeKeuanganReadModel> MetodeKeuanganReadModels => Set<MetodeKeuanganReadModel>();
    public DbSet<GlobalLeasingReadModel> GlobalLeasingReadModels => Set<GlobalLeasingReadModel>();
    public DbSet<CabangLeasingReadModel> CabangLeasingReadModels => Set<CabangLeasingReadModel>();
    public DbSet<GrupTenorReadModel> GrupTenorReadModels => Set<GrupTenorReadModel>();
    public DbSet<TenorReadModel> TenorReadModels => Set<TenorReadModel>();
    public DbSet<DfReadModel> DfReadModels => Set<DfReadModel>();
    public DbSet<DaftarHargaLeasingReadModel> DaftarHargaLeasingReadModels => Set<DaftarHargaLeasingReadModel>();
    public DbSet<DaftarHargaLeasingItemReadModel> DaftarHargaLeasingItemReadModels => Set<DaftarHargaLeasingItemReadModel>();
    public DbSet<DiscountReadModel> DiscountReadModels => Set<DiscountReadModel>();
    public DbSet<DiscountItemReadModel> DiscountItemReadModels => Set<DiscountItemReadModel>();
    public DbSet<DiscountCashReadModel> DiscountCashReadModels => Set<DiscountCashReadModel>();
    public DbSet<AlokasiDiskonReadModel> AlokasiDiskonReadModels => Set<AlokasiDiskonReadModel>();

    // Phase 4: Service Bureau & Finance Config
    public DbSet<SamsatReadModel> SamsatReadModels => Set<SamsatReadModel>();

    public DbSet<SamsatCityReadModel> SamsatCityReadModels => Set<SamsatCityReadModel>();
    public DbSet<BiroReadModel> BiroReadModels => Set<BiroReadModel>();
    public DbSet<BiayaBiroJasaReadModel> BiayaBiroJasaReadModels => Set<BiayaBiroJasaReadModel>();
    public DbSet<BiayaBiroJasaItemReadModel> BiayaBiroJasaItemReadModels => Set<BiayaBiroJasaItemReadModel>();
    public DbSet<ExpenseTypeReadModel> ExpenseTypeReadModels => Set<ExpenseTypeReadModel>();
    public DbSet<LedgerReadModel> LedgerReadModels => Set<LedgerReadModel>();
    public DbSet<PelanggaranWilayahReadModel> PelanggaranWilayahReadModels => Set<PelanggaranWilayahReadModel>();

    // Phase 6: Sales
    public DbSet<RegistrasiPenjualanReadModel> RegistrasiPenjualanReadModels => Set<RegistrasiPenjualanReadModel>();
    public DbSet<PengirimanMotorReadModel> PengirimanMotorReadModels => Set<PengirimanMotorReadModel>();

    // Phase 7: Document Workflows
    public DbSet<StnkReadModel> StnkReadModels => Set<StnkReadModel>();
    public DbSet<BpkbReadModel> BpkbReadModels => Set<BpkbReadModel>();

    // Phase 8: Finance & Accounting
    public DbSet<ArTransactionReadModel> ArTransactions => Set<ArTransactionReadModel>();
    public DbSet<ApTransactionReadModel> ApTransactions => Set<ApTransactionReadModel>();
    public DbSet<CashOutTransactionReadModel> CashOutTransactions => Set<CashOutTransactionReadModel>();
    public DbSet<FinanceCounterReadModel> FinanceCounters => Set<FinanceCounterReadModel>();

    // Phase 5: Inventory & Stock
    public DbSet<SoReadModel> SoReadModels => Set<SoReadModel>();
    public DbSet<SoItemReadModel> SoItemReadModels => Set<SoItemReadModel>();
    public DbSet<StockReadModel> StockReadModels => Set<StockReadModel>();
    public DbSet<SoPenerimaanReadModel> SoPenerimaanReadModels => Set<SoPenerimaanReadModel>();
    public DbSet<SoPenerimaanItemMotorReadModel> SoPenerimaanItemMotorReadModels => Set<SoPenerimaanItemMotorReadModel>();
    public DbSet<SoPenerimaanItemKelengkapanReadModel> SoPenerimaanItemKelengkapanReadModels => Set<SoPenerimaanItemKelengkapanReadModel>();
    public DbSet<SoReturReadModel> SoReturReadModels => Set<SoReturReadModel>();
    public DbSet<SoReturItemReadModel> SoReturItemReadModels => Set<SoReturItemReadModel>();
    public DbSet<MutasiReadModel> MutasiReadModels => Set<MutasiReadModel>();
    public DbSet<MutasiItemReadModel> MutasiItemReadModels => Set<MutasiItemReadModel>();
    public DbSet<MutasiKelengkapanReadModel> MutasiKelengkapanReadModels => Set<MutasiKelengkapanReadModel>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ── Event store ─────────────────────────────────────────────────────────
        // NOT dealer-filtered: the event store is queried directly, and repositories add
        // an explicit predicate on load. The unique index gives per-dealer optimistic
        // concurrency on (aggregate, version).
        builder.Entity<StoredEvent>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.DealerId, x.AggregateType, x.AggregateId, x.Version }).IsUnique();
            e.HasIndex(x => new { x.DealerId, x.AggregateId, x.AggregateType });
        });

        // ── Dealer registry (platform-level — intentionally NOT dealer-filtered) ──
        builder.Entity<DealerReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Code).HasMaxLength(30).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Code).IsUnique();
        });

        // ── Dealer-scoped read models ───────────────────────────────────────────
        // Null-tolerant filter: when DealerId is null (SuperAdmin / migrations / background),
        // no filter is applied and all rows are visible.
        builder.Entity<JabatanReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.DealerId, x.Name }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<KaryawanReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.KtpNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.Gender).HasMaxLength(10).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            e.Property(x => x.PhoneAlt).HasMaxLength(20);
            e.Property(x => x.Address).HasMaxLength(500);
            e.Property(x => x.SalesLimit).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.DealerId, x.Email }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<KiosReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            e.Property(x => x.Fax).HasMaxLength(20);
            e.Property(x => x.Address).HasMaxLength(500);
            e.Property(x => x.Limit).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<MediatorReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Address).HasMaxLength(500);
            e.Property(x => x.Limit).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Name }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<WarnaReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<GrupTipeMotorReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<TipeMotorReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.ShortName).HasMaxLength(100).IsRequired();
            e.Property(x => x.ProductCode).HasMaxLength(100);
            e.Property(x => x.WmsCode).HasMaxLength(100);
            e.Property(x => x.AhmCode).HasMaxLength(100);
            e.Property(x => x.EngineNumberFormat).HasMaxLength(100);
            e.Property(x => x.ChassisNumberFormat).HasMaxLength(100);
            e.Property(x => x.NettPrice).HasColumnType("numeric(18,2)");
            e.Property(x => x.OrJakarta).HasColumnType("numeric(18,2)");
            e.Property(x => x.OrTangerang).HasColumnType("numeric(18,2)");
            e.Property(x => x.BbnJakarta).HasColumnType("numeric(18,2)");
            e.Property(x => x.BbnTangerang).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<TipeMotorWarnaReadModel>(e =>
        {
            e.HasKey(x => new { x.TipeMotorId, x.WarnaId });
        });

        builder.Entity<KelengkapanReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Name }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        // ── Phase 3: Leasing & Financing ───────────────────────────────────────

        builder.Entity<MetodeKeuanganReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<GlobalLeasingReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30).IsRequired();
            e.Property(x => x.Fax).HasMaxLength(30);
            e.Property(x => x.Contact).HasMaxLength(100).IsRequired();
            e.Property(x => x.Address).HasMaxLength(500);
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<CabangLeasingReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.Fax).HasMaxLength(30);
            e.Property(x => x.Contact).HasMaxLength(100);
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<GrupTenorReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<TenorReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<DfReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Discount).HasColumnType("numeric(18,2)");
            e.Property(x => x.Interest).HasColumnType("numeric(18,2)");
            e.Property(x => x.UpdatedBy).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.DealerId).IsUnique();
            // No query filter — unique by DealerId, queried manually by DealerId
        });

        builder.Entity<DaftarHargaLeasingReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Name, x.GlobalLeasingId, x.GrupTenorId }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<DaftarHargaLeasingItemReadModel>(e =>
        {
            e.HasKey(x => new { x.DaftarHargaLeasingId, x.GrupTipeMotorId });
            e.Property(x => x.Subsidi).HasColumnType("numeric(18,2)");
            e.Property(x => x.Incentive).HasColumnType("numeric(18,2)");
            e.Property(x => x.LainLain).HasColumnType("numeric(18,2)");
            // No query filter — accessed via parent DaftarHargaLeasing
        });

        builder.Entity<DiscountReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Level).HasMaxLength(20).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.DaftarHargaLeasingId, x.Level }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<DiscountItemReadModel>(e =>
        {
            e.HasKey(x => new { x.DiscountId, x.GrupTipeMotorId });
            e.Property(x => x.Amount).HasColumnType("numeric(18,2)");
            // No query filter — accessed via parent Discount
        });

        builder.Entity<DiscountCashReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.DirectDiscount).HasColumnType("numeric(18,2)");
            e.Property(x => x.ChannelDiscount).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.TipeMotorId }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<AlokasiDiskonReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.DiscountLevel).HasMaxLength(20).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.KaryawanId }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        // ── Phase 4: Service Bureau & Finance Config ───────────────────────────

        builder.Entity<SamsatReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Name }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<SamsatCityReadModel>(e =>
        {
            e.HasKey(x => new { x.SamsatId, x.City });
            e.Property(x => x.City).HasMaxLength(200).IsRequired();
            // No dealer query filter — accessed via parent Samsat
        });

        builder.Entity<BiroReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.Fax).HasMaxLength(30);
            e.Property(x => x.Pic).HasMaxLength(100);
            e.Property(x => x.Address).HasMaxLength(500);
            e.Property(x => x.PphRate).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<BiayaBiroJasaReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.SamsatId, x.BiroId }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<BiayaBiroJasaItemReadModel>(e =>
        {
            e.HasKey(x => new { x.BiayaBiroJasaId, x.TipeMotorId });
            e.Property(x => x.BiayaStnk).HasColumnType("numeric(18,2)");
            e.Property(x => x.Notice).HasColumnType("numeric(18,2)");
            // No dealer query filter — accessed via parent BiayaBiroJasa
        });

        builder.Entity<ExpenseTypeReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Code }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<LedgerReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.Name }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<PelanggaranWilayahReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.AreaCode).HasMaxLength(50).IsRequired();
            e.Property(x => x.ExtraFee).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.AreaCode }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        // ── Phase 5: Inventory & Stock ─────────────────────────────────────────

        builder.Entity<SoReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SoNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.PaymentType).HasMaxLength(10).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20).IsRequired();
            e.Property(x => x.Total).HasColumnType("numeric(18,2)");
            e.Property(x => x.Subsidi).HasColumnType("numeric(18,2)");
            e.Property(x => x.CashDiscount).HasColumnType("numeric(18,2)");
            e.Property(x => x.PPn).HasColumnType("numeric(18,2)");
            e.Property(x => x.TotalAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.Df).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.SoNumber }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<SoItemReadModel>(e =>
        {
            e.HasKey(x => new { x.SoId, x.TipeMotorId, x.WarnaId });
            e.Property(x => x.NettPrice).HasColumnType("numeric(18,2)");
            // No dealer query filter — accessed via parent So
        });

        builder.Entity<StockReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.NoMesin).HasMaxLength(100).IsRequired();
            e.Property(x => x.NoRangka).HasMaxLength(100).IsRequired();
            e.Property(x => x.SuratJalanId).HasMaxLength(50).IsRequired();
            e.Property(x => x.AssemblyYear).HasMaxLength(10).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.NoMesin }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<SoPenerimaanReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SuratJalanId).HasMaxLength(50).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.DealerId, x.SuratJalanId }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<SoPenerimaanItemMotorReadModel>(e =>
        {
            e.HasKey(x => new { x.SoPenerimaanId, x.NoMesin });
            e.Property(x => x.NoMesin).HasMaxLength(100).IsRequired();
            e.Property(x => x.NoRangka).HasMaxLength(100).IsRequired();
            e.Property(x => x.AssemblyYear).HasMaxLength(10).IsRequired();
            // No dealer query filter — accessed via parent SoPenerimaan
        });

        builder.Entity<SoPenerimaanItemKelengkapanReadModel>(e =>
        {
            e.HasKey(x => new { x.SoPenerimaanId, x.KelengkapanId });
            // No dealer query filter — accessed via parent SoPenerimaan
        });

        builder.Entity<SoReturReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ReturNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.Reason).HasMaxLength(500).IsRequired();
            e.Property(x => x.Total).HasColumnType("numeric(18,2)");
            e.Property(x => x.PPn).HasColumnType("numeric(18,2)");
            e.Property(x => x.TotalAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.DealerId, x.ReturNumber }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<SoReturItemReadModel>(e =>
        {
            e.HasKey(x => new { x.SoReturId, x.TipeMotorId, x.WarnaId });
            e.Property(x => x.NettPrice).HasColumnType("numeric(18,2)");
            // No dealer query filter — accessed via parent SoRetur
        });

        builder.Entity<MutasiReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.MutasiNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasIndex(x => new { x.DealerId, x.MutasiNumber }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<MutasiItemReadModel>(e =>
        {
            e.HasKey(x => new { x.MutasiId, x.NoMesin });
            e.Property(x => x.NoMesin).HasMaxLength(100).IsRequired();
            // No dealer query filter — accessed via parent Mutasi
        });

        builder.Entity<MutasiKelengkapanReadModel>(e =>
        {
            e.HasKey(x => new { x.MutasiId, x.KelengkapanName });
            e.Property(x => x.KelengkapanName).HasMaxLength(200).IsRequired();
            // No dealer query filter — accessed via parent Mutasi
        });

        // ── Phase 6: Sales ─────────────────────────────────────────────────────

        builder.Entity<RegistrasiPenjualanReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.NoPenjualan).HasMaxLength(50).IsRequired();
            e.Property(x => x.MetodePenjualan).HasMaxLength(10).IsRequired();
            e.Property(x => x.TipePenjualan).HasMaxLength(20).IsRequired();
            e.Property(x => x.NoMesin).HasMaxLength(100).IsRequired();
            e.Property(x => x.NoRangka).HasMaxLength(100).IsRequired();
            e.Property(x => x.NamaCustomer).HasMaxLength(200).IsRequired();
            e.Property(x => x.Address).HasMaxLength(500);
            e.Property(x => x.Phone).HasMaxLength(30).IsRequired();
            e.Property(x => x.Phone1).HasMaxLength(30);
            e.Property(x => x.Phone2).HasMaxLength(30);
            e.Property(x => x.OffRoad).HasColumnType("numeric(18,2)");
            e.Property(x => x.Bbn).HasColumnType("numeric(18,2)");
            e.Property(x => x.Discount).HasColumnType("numeric(18,2)");
            e.Property(x => x.ApprovedDiscount).HasColumnType("numeric(18,2)");
            e.Property(x => x.OriginalDiscount).HasColumnType("numeric(18,2)");
            e.Property(x => x.Total).HasColumnType("numeric(18,2)");
            e.Property(x => x.AmbilUang).HasColumnType("numeric(18,2)");
            e.Property(x => x.Dp).HasColumnType("numeric(18,2)");
            e.Property(x => x.Angsuran).HasColumnType("numeric(18,2)");
            e.Property(x => x.Tac).HasColumnType("numeric(18,2)");
            e.Property(x => x.TenorCode).HasMaxLength(50);
            e.Property(x => x.TipeMotorCode).HasMaxLength(100).IsRequired();
            e.Property(x => x.WarnaName).HasMaxLength(100).IsRequired();
            e.Property(x => x.SerahTerimaKendaraanId).HasMaxLength(100).IsRequired();
            e.Property(x => x.TandaTerimaSementaraId).HasMaxLength(100);
            e.Property(x => x.Kelengkapan).HasMaxLength(1000);
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => new { x.DealerId, x.NoPenjualan }).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<PengirimanMotorReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.NoMesin).HasMaxLength(100).IsRequired();
            e.Property(x => x.Zona).HasMaxLength(100);
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        // ── Phase 7: Document Workflows ────────────────────────────────────────

        builder.Entity<StnkReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasMaxLength(20).IsRequired();
            e.Property(x => x.FakturName).HasMaxLength(200).IsRequired();
            e.Property(x => x.FakturAddress).HasMaxLength(500).IsRequired();
            e.Property(x => x.InvoiceNumber).HasMaxLength(100);
            e.Property(x => x.PlateNumber).HasMaxLength(20);
            e.Property(x => x.StnkNumber).HasMaxLength(100);
            e.Property(x => x.Region).HasMaxLength(100);
            e.Property(x => x.StnkCost).HasColumnType("numeric(18,2)");
            e.Property(x => x.ProgressiveCost).HasColumnType("numeric(18,2)");
            e.Property(x => x.NoticeCost).HasColumnType("numeric(18,2)");
            e.Property(x => x.BbnCost).HasColumnType("numeric(18,2)");
            e.Property(x => x.PnbpCost).HasColumnType("numeric(18,2)");
            e.Property(x => x.AdminCost).HasColumnType("numeric(18,2)");
            e.Property(x => x.OtherCost).HasColumnType("numeric(18,2)");
            e.Property(x => x.ServiceCost).HasColumnType("numeric(18,2)");
            e.Property(x => x.PphCost).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => x.RegistrasiPenjualanId).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<BpkbReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasMaxLength(20).IsRequired();
            e.Property(x => x.BpkbNumber).HasMaxLength(100);
            e.Property(x => x.BookNumber).HasMaxLength(100);
            e.Property(x => x.Receiver).HasMaxLength(200);
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.UpdatedBy).HasMaxLength(100);
            e.HasIndex(x => x.StnkId).IsUnique();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        // ── Phase 8: Finance & Accounting ──────────────────────────────────────

        builder.Entity<ArTransactionReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TransactionType).HasMaxLength(30).IsRequired();
            e.Property(x => x.ReferenceNumber).HasMaxLength(100).IsRequired();
            e.Property(x => x.TotalAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.RemainingAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
            e.OwnsMany(x => x.Payments, p =>
            {
                p.WithOwner().HasForeignKey("ArTransactionId");
                p.Property<Guid>("ArTransactionId");
                p.HasKey("ArTransactionId", nameof(ArPaymentEntryReadModel.PaymentNo));
                p.Property(x => x.Amount).HasColumnType("numeric(18,2)");
                p.Property(x => x.PaymentMethod).HasMaxLength(50).IsRequired();
                p.Property(x => x.FInvoiceId).HasMaxLength(50).IsRequired();
                p.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            });
        });

        builder.Entity<ApTransactionReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TransactionType).HasMaxLength(30).IsRequired();
            e.Property(x => x.NoRangka).HasMaxLength(100).IsRequired();
            e.Property(x => x.TotalAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.RemainingAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
            e.OwnsMany(x => x.Payments, p =>
            {
                p.WithOwner().HasForeignKey("ApTransactionId");
                p.Property<Guid>("ApTransactionId");
                p.HasKey("ApTransactionId", nameof(ApPaymentEntryReadModel.PaymentNo));
                p.Property(x => x.Amount).HasColumnType("numeric(18,2)");
                p.Property(x => x.PaymentMethod).HasMaxLength(50).IsRequired();
                p.Property(x => x.FInvoiceId).HasMaxLength(50).IsRequired();
                p.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            });
        });

        builder.Entity<CashOutTransactionReadModel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.TransactionType).HasMaxLength(30).IsRequired();
            e.Property(x => x.Amount).HasColumnType("numeric(18,2)");
            e.Property(x => x.DfAmount).HasColumnType("numeric(18,2)");
            e.Property(x => x.PaymentMethod).HasMaxLength(50).IsRequired();
            e.Property(x => x.FInvoiceId).HasMaxLength(50).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.HasQueryFilter(x => CurrentDealerId == null || x.DealerId == CurrentDealerId);
        });

        builder.Entity<FinanceCounterReadModel>(e =>
        {
            e.HasKey(x => x.DealerId);
            // No global query filter — keyed by DealerId, not dealer-scoped content
        });
    }
}
