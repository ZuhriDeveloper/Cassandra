using Cassandra.Application.Contracts.Dealers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cassandra.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so <c>dotnet ef</c> can construct the context without the full app
/// DI graph. The connection string is a placeholder — scaffolding a migration only needs the
/// model, not a live database.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=cassandradb;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options, new NullCurrentDealer());
    }

    // Returns null for all dealer properties — global query filters become pass-through.
    private sealed class NullCurrentDealer : ICurrentDealer
    {
        public Guid DealerId => throw new InvalidOperationException("No dealer in design-time context.");
        public Guid? DealerIdOrNull => null;
        public bool IsSuperAdmin => false;
    }
}
