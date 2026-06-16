using Cassandra.Application.Commands.CabangLeasing.CreateCabangLeasing;
using Cassandra.Application.Contracts.CabangLeasing;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.DTOs.CabangLeasing;
using Cassandra.Domain.CabangLeasing;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.CabangLeasing;

public class CreateCabangLeasingCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid GlobalLeasingId = Guid.NewGuid();

    private static CreateCabangLeasingCommand DefaultCommand(string code = "BCA-JKT") =>
        new(code, "BCA Jakarta", "021-5555", null, "John", GlobalLeasingId, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeAndNameAreUnique()
    {
        var repo = new FakeCabangLeasingRepository();
        var query = new FakeCabangLeasingQueryRepository { CodeExists = false, NameExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateCabangLeasingCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("BCA-JKT", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeCabangLeasingRepository();
        var query = new FakeCabangLeasingQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateCabangLeasingCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenNameExists()
    {
        var repo = new FakeCabangLeasingRepository();
        var query = new FakeCabangLeasingQueryRepository { CodeExists = false, NameExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateCabangLeasingCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeCabangLeasingRepository : ICabangLeasingRepository
    {
        public Domain.CabangLeasing.CabangLeasing? Saved { get; private set; }

        public Task<Domain.CabangLeasing.CabangLeasing?> GetByIdAsync(CabangLeasingId id, CancellationToken ct = default)
            => Task.FromResult<Domain.CabangLeasing.CabangLeasing?>(null);

        public Task SaveAsync(Domain.CabangLeasing.CabangLeasing cabangLeasing, CancellationToken ct = default)
        {
            Saved = cabangLeasing;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeCabangLeasingQueryRepository : ICabangLeasingQueryRepository
    {
        public bool CodeExists { get; init; }
        public bool NameExists { get; init; }

        public Task<CabangLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<CabangLeasingDto?>(null);

        public Task<IReadOnlyList<CabangLeasingDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<CabangLeasingDto>>([]);

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
