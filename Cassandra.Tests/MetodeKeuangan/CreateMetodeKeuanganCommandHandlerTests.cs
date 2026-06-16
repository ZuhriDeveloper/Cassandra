using Cassandra.Application.Commands.MetodeKeuangan.CreateMetodeKeuangan;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.MetodeKeuangan;
using Cassandra.Application.DTOs.MetodeKeuangan;
using Cassandra.Domain.Common;
using Cassandra.Domain.MetodeKeuangan;

namespace Cassandra.Tests.MetodeKeuangan;

public class CreateMetodeKeuanganCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateMetodeKeuanganCommand DefaultCommand(string code = "KAS") =>
        new(code, "Kas", "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeIsUnique()
    {
        var repo = new FakeMetodeKeuanganRepository();
        var query = new FakeMetodeKeuanganQueryRepository { CodeExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateMetodeKeuanganCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("KAS", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeMetodeKeuanganRepository();
        var query = new FakeMetodeKeuanganQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateMetodeKeuanganCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeMetodeKeuanganRepository : IMetodeKeuanganRepository
    {
        public Domain.MetodeKeuangan.MetodeKeuangan? Saved { get; private set; }

        public Task<Domain.MetodeKeuangan.MetodeKeuangan?> GetByIdAsync(MetodeKeuanganId id, CancellationToken ct = default)
            => Task.FromResult<Domain.MetodeKeuangan.MetodeKeuangan?>(null);

        public Task SaveAsync(Domain.MetodeKeuangan.MetodeKeuangan mk, CancellationToken ct = default)
        {
            Saved = mk;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeMetodeKeuanganQueryRepository : IMetodeKeuanganQueryRepository
    {
        public bool CodeExists { get; init; }

        public Task<MetodeKeuanganDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<MetodeKeuanganDto?>(null);

        public Task<IReadOnlyList<MetodeKeuanganDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<MetodeKeuanganDto>>([]);

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => Task.FromResult(CodeExists);

        public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
            => Task.FromResult(false);

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
