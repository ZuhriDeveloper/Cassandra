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
    }
}
