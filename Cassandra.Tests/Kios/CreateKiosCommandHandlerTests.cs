using Cassandra.Application.Commands.Kios.CreateKios;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Kios;
using Cassandra.Application.DTOs.Kios;
using Cassandra.Domain.Common;
using Cassandra.Domain.Kios;

namespace Cassandra.Tests.Kios;

public class CreateKiosCommandHandlerTests
{
    private static readonly Guid DealerId      = Guid.NewGuid();
    private static readonly Guid PicKaryawanId = Guid.NewGuid();

    private static CreateKiosCommand DefaultCommand(string code = "K001") =>
        new(code, "Kios Utama", "021-12345", null, "Jl. Merdeka 1", PicKaryawanId, 0m, "admin");

    [Fact]
    public async Task Creates_kios_and_returns_id()
    {
        var repo    = new FakeKiosRepository();
        var query   = new FakeKiosQueryRepository { CodeExists = false };
        var dealer  = new FakeCurrentDealer(DealerId);
        var handler = new CreateKiosCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("K001", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task Throws_when_code_already_exists()
    {
        var repo    = new FakeKiosRepository();
        var query   = new FakeKiosQueryRepository { CodeExists = true };
        var dealer  = new FakeCurrentDealer(DealerId);
        var handler = new CreateKiosCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeKiosRepository : IKiosRepository
    {
        public Domain.Kios.Kios? Saved { get; private set; }

        public Task<Domain.Kios.Kios?> GetByIdAsync(KiosId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Kios.Kios?>(null);

        public Task SaveAsync(Domain.Kios.Kios kios, CancellationToken ct = default)
        {
            Saved = kios;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeKiosQueryRepository : IKiosQueryRepository
    {
        public bool CodeExists { get; init; }

        public Task<KiosDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<KiosDto?>(null);

        public Task<IReadOnlyList<KiosDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<KiosDto>>([]);

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => Task.FromResult(CodeExists);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid  DealerId      => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool  IsSuperAdmin   => false;
    }
}
