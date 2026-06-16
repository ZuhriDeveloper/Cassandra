using Cassandra.Application.Commands.Biro.CreateBiro;
using Cassandra.Application.Contracts.Biro;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.DTOs.Biro;
using Cassandra.Domain.Biro;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.Biro;

public class CreateBiroCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateBiroCommand DefaultCommand(string code = "BR001") =>
        new(code, "Biro Jasa Maju", null, null, null, null, 2.5m, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeIsUnique()
    {
        var repo = new FakeBiroRepository();
        var query = new FakeBiroQueryRepository { CodeExists = false, NameExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateBiroCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("BR001", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeBiroRepository();
        var query = new FakeBiroQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateBiroCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenNameExists()
    {
        var repo = new FakeBiroRepository();
        var query = new FakeBiroQueryRepository { CodeExists = false, NameExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateBiroCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeBiroRepository : IBiroRepository
    {
        public Domain.Biro.Biro? Saved { get; private set; }

        public Task<Domain.Biro.Biro?> GetByIdAsync(BiroId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Biro.Biro?>(null);

        public Task SaveAsync(Domain.Biro.Biro biro, CancellationToken ct = default)
        {
            Saved = biro;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeBiroQueryRepository : IBiroQueryRepository
    {
        public bool CodeExists { get; init; }
        public bool NameExists { get; init; }

        public Task<BiroDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<BiroDto?>(null);

        public Task<IReadOnlyList<BiroDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<BiroDto>>([]);

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => Task.FromResult(CodeExists);

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
