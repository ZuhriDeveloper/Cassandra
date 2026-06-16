using Cassandra.Application.Commands.DaftarHargaLeasing.CreateDaftarHargaLeasing;
using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.DTOs.DaftarHargaLeasing;
using Cassandra.Domain.Common;
using Cassandra.Domain.DaftarHargaLeasing;

namespace Cassandra.Tests.DaftarHargaLeasing;

public class CreateDaftarHargaLeasingCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid GlobalLeasingId = Guid.NewGuid();
    private static readonly Guid GrupTenorId = Guid.NewGuid();

    private static CreateDaftarHargaLeasingCommand DefaultCommand() =>
        new("DHL 2024", GlobalLeasingId, GrupTenorId, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCombinationIsUnique()
    {
        var repo = new FakeDhlRepository();
        var query = new FakeDhlQueryRepository { Exists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateDaftarHargaLeasingCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("DHL 2024", repo.Saved!.Name);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCombinationExists()
    {
        var repo = new FakeDhlRepository();
        var query = new FakeDhlQueryRepository { Exists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateDaftarHargaLeasingCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeDhlRepository : IDaftarHargaLeasingRepository
    {
        public Domain.DaftarHargaLeasing.DaftarHargaLeasing? Saved { get; private set; }

        public Task<Domain.DaftarHargaLeasing.DaftarHargaLeasing?> GetByIdAsync(DaftarHargaLeasingId id, CancellationToken ct = default)
            => Task.FromResult<Domain.DaftarHargaLeasing.DaftarHargaLeasing?>(null);

        public Task SaveAsync(Domain.DaftarHargaLeasing.DaftarHargaLeasing daftarHargaLeasing, CancellationToken ct = default)
        {
            Saved = daftarHargaLeasing;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeDhlQueryRepository : IDaftarHargaLeasingQueryRepository
    {
        public bool Exists { get; init; }

        public Task<DaftarHargaLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<DaftarHargaLeasingDto?>(null);

        public Task<IReadOnlyList<DaftarHargaLeasingDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<DaftarHargaLeasingDto>>([]);

        public Task<bool> ExistsAsync(string name, Guid globalLeasingId, Guid grupTenorId, CancellationToken ct = default)
            => Task.FromResult(Exists);

        public Task<bool> ExistsExcludingAsync(string name, Guid globalLeasingId, Guid grupTenorId, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
