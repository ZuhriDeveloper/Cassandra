using Cassandra.Application.Commands.Tenor.CreateTenor;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Tenor;
using Cassandra.Application.DTOs.Tenor;
using Cassandra.Domain.Common;
using Cassandra.Domain.Tenor;

namespace Cassandra.Tests.Tenor;

public class CreateTenorCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid GrupTenorId = Guid.NewGuid();

    private static CreateTenorCommand DefaultCommand(string code = "T12") =>
        new(code, "12 Bulan", 12, GrupTenorId, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeIsUnique()
    {
        var repo = new FakeTenorRepository();
        var query = new FakeTenorQueryRepository { CodeExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateTenorCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("T12", repo.Saved!.Code);
        Assert.Equal(12, repo.Saved.Months);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeTenorRepository();
        var query = new FakeTenorQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateTenorCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeTenorRepository : ITenorRepository
    {
        public Domain.Tenor.Tenor? Saved { get; private set; }

        public Task<Domain.Tenor.Tenor?> GetByIdAsync(TenorId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Tenor.Tenor?>(null);

        public Task SaveAsync(Domain.Tenor.Tenor tenor, CancellationToken ct = default)
        {
            Saved = tenor;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeTenorQueryRepository : ITenorQueryRepository
    {
        public bool CodeExists { get; init; }

        public Task<TenorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<TenorDto?>(null);

        public Task<IReadOnlyList<TenorDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<TenorDto>>([]);

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
