using Cassandra.Application.Commands.AlokasiDiskon.CreateAlokasiDiskon;
using Cassandra.Application.Contracts.AlokasiDiskon;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.DTOs.AlokasiDiskon;
using Cassandra.Domain.AlokasiDiskon;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.AlokasiDiskon;

public class CreateAlokasiDiskonCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid KaryawanId = Guid.NewGuid();

    private static CreateAlokasiDiskonCommand DefaultCommand() =>
        new(KaryawanId, "GOLD", "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenKaryawanIdIsUnique()
    {
        var repo = new FakeAlokasiDiskonRepository();
        var query = new FakeAlokasiDiskonQueryRepository { KaryawanExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateAlokasiDiskonCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal(KaryawanId, repo.Saved!.KaryawanId);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenKaryawanAlreadyHasAlokasi()
    {
        var repo = new FakeAlokasiDiskonRepository();
        var query = new FakeAlokasiDiskonQueryRepository { KaryawanExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateAlokasiDiskonCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeAlokasiDiskonRepository : IAlokasiDiskonRepository
    {
        public Domain.AlokasiDiskon.AlokasiDiskon? Saved { get; private set; }

        public Task<Domain.AlokasiDiskon.AlokasiDiskon?> GetByIdAsync(AlokasiDiskonId id, CancellationToken ct = default)
            => Task.FromResult<Domain.AlokasiDiskon.AlokasiDiskon?>(null);

        public Task SaveAsync(Domain.AlokasiDiskon.AlokasiDiskon alokasiDiskon, CancellationToken ct = default)
        {
            Saved = alokasiDiskon;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAlokasiDiskonQueryRepository : IAlokasiDiskonQueryRepository
    {
        public bool KaryawanExists { get; init; }

        public Task<AlokasiDiskonDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<AlokasiDiskonDto?>(null);

        public Task<IReadOnlyList<AlokasiDiskonDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<AlokasiDiskonDto>>([]);

        public Task<bool> KaryawanIdExistsAsync(Guid karyawanId, CancellationToken ct = default)
            => Task.FromResult(KaryawanExists);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
