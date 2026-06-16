using Cassandra.Application.Commands.BiayaBiroJasa.CreateBiayaBiroJasa;
using Cassandra.Application.Contracts.BiayaBiroJasa;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.DTOs.BiayaBiroJasa;
using Cassandra.Domain.BiayaBiroJasa;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.BiayaBiroJasa;

public class CreateBiayaBiroJasaCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid SamsatId = Guid.NewGuid();
    private static readonly Guid BiroId = Guid.NewGuid();

    private static CreateBiayaBiroJasaCommand DefaultCommand()
        => new(SamsatId, BiroId, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCombinationIsUnique()
    {
        var repo = new FakeBiayaBiroJasaRepository();
        var query = new FakeBiayaBiroJasaQueryRepository { SamsatBiroExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateBiayaBiroJasaCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal(SamsatId, repo.Saved!.SamsatId);
        Assert.Equal(BiroId, repo.Saved.BiroId);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCombinationExists()
    {
        var repo = new FakeBiayaBiroJasaRepository();
        var query = new FakeBiayaBiroJasaQueryRepository { SamsatBiroExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateBiayaBiroJasaCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeBiayaBiroJasaRepository : IBiayaBiroJasaRepository
    {
        public Domain.BiayaBiroJasa.BiayaBiroJasa? Saved { get; private set; }

        public Task<Domain.BiayaBiroJasa.BiayaBiroJasa?> GetByIdAsync(BiayaBiroJasaId id, CancellationToken ct = default)
            => Task.FromResult<Domain.BiayaBiroJasa.BiayaBiroJasa?>(null);

        public Task SaveAsync(Domain.BiayaBiroJasa.BiayaBiroJasa bbj, CancellationToken ct = default)
        {
            Saved = bbj;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeBiayaBiroJasaQueryRepository : IBiayaBiroJasaQueryRepository
    {
        public bool SamsatBiroExists { get; init; }

        public Task<BiayaBiroJasaDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<BiayaBiroJasaDto?>(null);

        public Task<IReadOnlyList<BiayaBiroJasaDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<BiayaBiroJasaDto>>([]);

        public Task<bool> SamsatBiroExistsAsync(Guid samsatId, Guid biroId, CancellationToken ct = default)
            => Task.FromResult(SamsatBiroExists);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
