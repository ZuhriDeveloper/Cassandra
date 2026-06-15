using Cassandra.Application.Commands.Kelengkapan.CreateKelengkapan;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Kelengkapan;
using Cassandra.Application.DTOs.Kelengkapan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Kelengkapan;

namespace Cassandra.Tests.Kelengkapan;

public class CreateKelengkapanCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateKelengkapanCommand DefaultCommand(string name = "Helm") =>
        new(name, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenNameIsUnique()
    {
        var repo = new FakeKelengkapanRepository();
        var query = new FakeKelengkapanQueryRepository { NameExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateKelengkapanCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("Helm", repo.Saved!.Name);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenNameExists()
    {
        var repo = new FakeKelengkapanRepository();
        var query = new FakeKelengkapanQueryRepository { NameExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateKelengkapanCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeKelengkapanRepository : IKelengkapanRepository
    {
        public Domain.Kelengkapan.Kelengkapan? Saved { get; private set; }

        public Task<Domain.Kelengkapan.Kelengkapan?> GetByIdAsync(KelengkapanId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Kelengkapan.Kelengkapan?>(null);

        public Task SaveAsync(Domain.Kelengkapan.Kelengkapan kelengkapan, CancellationToken ct = default)
        {
            Saved = kelengkapan;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeKelengkapanQueryRepository : IKelengkapanQueryRepository
    {
        public bool NameExists { get; init; }

        public Task<KelengkapanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<KelengkapanDto?>(null);

        public Task<IReadOnlyList<KelengkapanDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<KelengkapanDto>>([]);

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
