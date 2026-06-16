using Cassandra.Application.Commands.PelanggaranWilayah.CreatePelanggaranWilayah;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.PelanggaranWilayah;
using Cassandra.Application.DTOs.PelanggaranWilayah;
using Cassandra.Domain.Common;
using Cassandra.Domain.PelanggaranWilayah;

namespace Cassandra.Tests.PelanggaranWilayah;

public class CreatePelanggaranWilayahCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreatePelanggaranWilayahCommand DefaultCommand(string areaCode = "021")
        => new(areaCode, 500000m, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenAreaCodeIsUnique()
    {
        var repo = new FakePelanggaranWilayahRepository();
        var query = new FakePelanggaranWilayahQueryRepository { AreaCodeExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreatePelanggaranWilayahCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("021", repo.Saved!.AreaCode);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenAreaCodeExists()
    {
        var repo = new FakePelanggaranWilayahRepository();
        var query = new FakePelanggaranWilayahQueryRepository { AreaCodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreatePelanggaranWilayahCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakePelanggaranWilayahRepository : IPelanggaranWilayahRepository
    {
        public Domain.PelanggaranWilayah.PelanggaranWilayah? Saved { get; private set; }

        public Task<Domain.PelanggaranWilayah.PelanggaranWilayah?> GetByIdAsync(PelanggaranWilayahId id, CancellationToken ct = default)
            => Task.FromResult<Domain.PelanggaranWilayah.PelanggaranWilayah?>(null);

        public Task SaveAsync(Domain.PelanggaranWilayah.PelanggaranWilayah pw, CancellationToken ct = default)
        {
            Saved = pw;
            return Task.CompletedTask;
        }
    }

    private sealed class FakePelanggaranWilayahQueryRepository : IPelanggaranWilayahQueryRepository
    {
        public bool AreaCodeExists { get; init; }

        public Task<PelanggaranWilayahDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<PelanggaranWilayahDto?>(null);

        public Task<IReadOnlyList<PelanggaranWilayahDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<PelanggaranWilayahDto>>([]);

        public Task<bool> AreaCodeExistsAsync(string areaCode, CancellationToken ct = default)
            => Task.FromResult(AreaCodeExists);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
