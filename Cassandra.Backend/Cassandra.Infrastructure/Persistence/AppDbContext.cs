using Cassandra.Infrastructure.Identity;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<StoredEvent> StoredEvents => Set<StoredEvent>();
    public DbSet<DealerReadModel> DealerReadModels => Set<DealerReadModel>();

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

        // NOTE: When the first dealer-scoped transactional read model is added, inject
        // ICurrentDealer into this context and apply the null-tolerant global query filter
        // pattern (x => CurrentDealerId == null || x.DealerId == CurrentDealerId) to it,
        // mirroring Materia's AppDbContext. The dealer registry and event store stay excluded.
    }
}
