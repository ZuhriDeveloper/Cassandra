using Cassandra.Application.Commands.GlobalLeasing.CreateGlobalLeasing;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.GlobalLeasing;
using Cassandra.Application.DTOs.GlobalLeasing;
using Cassandra.Domain.Common;
using Cassandra.Domain.GlobalLeasing;

namespace Cassandra.Tests.GlobalLeasing;

public class CreateGlobalLeasingCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateGlobalLeasingCommand DefaultCommand(string code = "BCA") =>
        new(code, "BCA Finance", "021-5555", null, "John", "Jakarta", "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeIsUnique()
    {
        var repo = new FakeGlobalLeasingRepository();
        var query = new FakeGlobalLeasingQueryRepository { CodeExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateGlobalLeasingCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("BCA", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeGlobalLeasingRepository();
        var query = new FakeGlobalLeasingQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateGlobalLeasingCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeGlobalLeasingRepository : IGlobalLeasingRepository
    {
        public Domain.GlobalLeasing.GlobalLeasing? Saved { get; private set; }

        public Task<Domain.GlobalLeasing.GlobalLeasing?> GetByIdAsync(GlobalLeasingId id, CancellationToken ct = default)
            => Task.FromResult<Domain.GlobalLeasing.GlobalLeasing?>(null);

        public Task SaveAsync(Domain.GlobalLeasing.GlobalLeasing gl, CancellationToken ct = default)
        {
            Saved = gl;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeGlobalLeasingQueryRepository : IGlobalLeasingQueryRepository
    {
        public bool CodeExists { get; init; }

        public Task<GlobalLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<GlobalLeasingDto?>(null);

        public Task<IReadOnlyList<GlobalLeasingDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<GlobalLeasingDto>>([]);

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
