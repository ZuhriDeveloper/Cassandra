using Cassandra.Application.Commands.Df.SetDf;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Df;

namespace Cassandra.Tests.Df;

public class SetDfCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static SetDfCommand DefaultCommand() =>
        new(5m, 3m, new DateOnly(2024, 1, 1), "admin");

    [Fact]
    public async Task HandleAsync_Creates_WhenNoDfExistsForDealer()
    {
        var repo = new FakeDfRepository { ExistingDf = null };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new SetDfCommandHandler(repo, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal(5m, repo.Saved!.Discount);
        Assert.Equal(3m, repo.Saved.Interest);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Updates_WhenDfAlreadyExistsForDealer()
    {
        var existing = Domain.Df.Df.Create(1m, 1m, new DateOnly(2023, 1, 1), "admin", DealerId);
        var repo = new FakeDfRepository { ExistingDf = existing };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new SetDfCommandHandler(repo, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.Equal(existing.Id.Value, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal(5m, repo.Saved!.Discount);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeDfRepository : IDfRepository
    {
        public Domain.Df.Df? ExistingDf { get; init; }
        public Domain.Df.Df? Saved { get; private set; }

        public Task<Domain.Df.Df?> GetForDealerAsync(Guid dealerId, CancellationToken ct = default)
            => Task.FromResult(ExistingDf);

        public Task SaveAsync(Domain.Df.Df df, CancellationToken ct = default)
        {
            Saved = df;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
