using Cassandra.Application.Commands.Warna.CreateWarna;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Warna;
using Cassandra.Application.DTOs.Warna;
using Cassandra.Domain.Common;
using Cassandra.Domain.Warna;

namespace Cassandra.Tests.Warna;

public class CreateWarnaCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateWarnaCommand DefaultCommand(string code = "MR") =>
        new(code, "Merah", "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeIsUnique()
    {
        var repo = new FakeWarnaRepository();
        var query = new FakeWarnaQueryRepository { CodeExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateWarnaCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("MR", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeWarnaRepository();
        var query = new FakeWarnaQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateWarnaCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeWarnaRepository : IWarnaRepository
    {
        public Domain.Warna.Warna? Saved { get; private set; }

        public Task<Domain.Warna.Warna?> GetByIdAsync(WarnaId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Warna.Warna?>(null);

        public Task SaveAsync(Domain.Warna.Warna warna, CancellationToken ct = default)
        {
            Saved = warna;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeWarnaQueryRepository : IWarnaQueryRepository
    {
        public bool CodeExists { get; init; }

        public Task<WarnaDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<WarnaDto?>(null);

        public Task<IReadOnlyList<WarnaDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<WarnaDto>>([]);

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => Task.FromResult(CodeExists);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
