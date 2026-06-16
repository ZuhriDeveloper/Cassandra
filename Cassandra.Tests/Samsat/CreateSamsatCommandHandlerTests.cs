using Cassandra.Application.Commands.Samsat.CreateSamsat;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Samsat;
using Cassandra.Application.DTOs.Samsat;
using Cassandra.Domain.Common;
using Cassandra.Domain.Samsat;

namespace Cassandra.Tests.Samsat;

public class CreateSamsatCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateSamsatCommand DefaultCommand(string name = "Samsat Jakarta Barat")
        => new(name, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenNameIsUnique()
    {
        var repo = new FakeSamsatRepository();
        var query = new FakeSamsatQueryRepository { NameExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateSamsatCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("Samsat Jakarta Barat", repo.Saved!.Name);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenNameExists()
    {
        var repo = new FakeSamsatRepository();
        var query = new FakeSamsatQueryRepository { NameExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateSamsatCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeSamsatRepository : ISamsatRepository
    {
        public Domain.Samsat.Samsat? Saved { get; private set; }

        public Task<Domain.Samsat.Samsat?> GetByIdAsync(SamsatId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Samsat.Samsat?>(null);

        public Task SaveAsync(Domain.Samsat.Samsat samsat, CancellationToken ct = default)
        {
            Saved = samsat;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeSamsatQueryRepository : ISamsatQueryRepository
    {
        public bool NameExists { get; init; }

        public Task<SamsatDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<SamsatDto?>(null);

        public Task<IReadOnlyList<SamsatDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<SamsatDto>>([]);

        public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
            => Task.FromResult(NameExists);

        public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
