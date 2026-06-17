using Cassandra.Application.Commands.Mutasi.CreateMutasi;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Mutasi;
using Cassandra.Application.Contracts.Stock;
using Cassandra.Application.DTOs.Mutasi;
using Cassandra.Application.DTOs.Stock;
using Cassandra.Domain.Common;
using Cassandra.Domain.Mutasi;
using Cassandra.Domain.Stock;

namespace Cassandra.Tests.Mutasi;

public class CreateMutasiCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid SourceKiosId = Guid.NewGuid();
    private static readonly Guid DestKiosId = Guid.NewGuid();

    private static CreateMutasiCommand DefaultCommand(string mutasiNumber = "MUT-001") =>
        new(
            mutasiNumber,
            DateOnly.FromDateTime(DateTime.Today),
            SourceKiosId,
            DestKiosId,
            ["M001"],
            [],
            "admin");

    private static Domain.Stock.Stock MakeStockWithStatus(string noMesin = "M001", string status = StockStatus.TERSEDIA)
    {
        var stock = Domain.Stock.Stock.Create(noMesin, "R001", Guid.NewGuid(), Guid.NewGuid(),
            SourceKiosId, "SJ-001", DateOnly.FromDateTime(DateTime.Today), Guid.NewGuid(), "2025", "admin", DealerId);
        if (status != StockStatus.TERSEDIA)
            stock.ChangeStatus(status, "admin");
        stock.ClearDomainEvents();
        return stock;
    }

    [Fact]
    public async Task HandleAsync_Succeeds_AndMovesStock()
    {
        var stock = MakeStockWithStatus();
        var repo = new FakeMutasiRepository();
        var queryRepo = new FakeMutasiQueryRepository { MutasiNumberExists = false };
        var stockRepo = new FakeStockRepository(stock);
        var stockQueryRepo = new FakeStockQueryRepository(stock);
        var dealer = new FakeCurrentDealer(DealerId);

        var handler = new CreateMutasiCommandHandler(repo, queryRepo, stockRepo, stockQueryRepo, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        // Stock should have been moved to destination kios
        Assert.Equal(DestKiosId, stock.KiosId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenMutasiNumberExists()
    {
        var stock = MakeStockWithStatus();
        var repo = new FakeMutasiRepository();
        var queryRepo = new FakeMutasiQueryRepository { MutasiNumberExists = true };
        var stockRepo = new FakeStockRepository(stock);
        var stockQueryRepo = new FakeStockQueryRepository(stock);
        var dealer = new FakeCurrentDealer(DealerId);

        var handler = new CreateMutasiCommandHandler(repo, queryRepo, stockRepo, stockQueryRepo, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenSameKios()
    {
        var stock = MakeStockWithStatus();
        var repo = new FakeMutasiRepository();
        var queryRepo = new FakeMutasiQueryRepository { MutasiNumberExists = false };
        var stockRepo = new FakeStockRepository(stock);
        var stockQueryRepo = new FakeStockQueryRepository(stock);
        var dealer = new FakeCurrentDealer(DealerId);

        var command = DefaultCommand() with { DestinationKiosId = SourceKiosId };
        var handler = new CreateMutasiCommandHandler(repo, queryRepo, stockRepo, stockQueryRepo, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(command, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenStockNotFound()
    {
        var repo = new FakeMutasiRepository();
        var queryRepo = new FakeMutasiQueryRepository { MutasiNumberExists = false };
        var stockRepo = new FakeStockRepository(null);
        var stockQueryRepo = new FakeStockQueryRepository(null);
        var dealer = new FakeCurrentDealer(DealerId);

        var handler = new CreateMutasiCommandHandler(repo, queryRepo, stockRepo, stockQueryRepo, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenStockNotTersedia()
    {
        var stock = MakeStockWithStatus(status: StockStatus.DIPESAN);
        var repo = new FakeMutasiRepository();
        var queryRepo = new FakeMutasiQueryRepository { MutasiNumberExists = false };
        var stockRepo = new FakeStockRepository(stock);
        var stockQueryRepo = new FakeStockQueryRepository(stock);
        var dealer = new FakeCurrentDealer(DealerId);

        var handler = new CreateMutasiCommandHandler(repo, queryRepo, stockRepo, stockQueryRepo, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    // ── Fakes ──────────────────────────────────────────────────────────────────

    private sealed class FakeMutasiRepository : IMutasiRepository
    {
        public Domain.Mutasi.Mutasi? Saved { get; private set; }

        public Task<Domain.Mutasi.Mutasi?> GetByIdAsync(MutasiId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Mutasi.Mutasi?>(null);

        public Task SaveAsync(Domain.Mutasi.Mutasi mutasi, CancellationToken ct = default)
        {
            Saved = mutasi;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeMutasiQueryRepository : IMutasiQueryRepository
    {
        public bool MutasiNumberExists { get; init; }

        public Task<IReadOnlyList<MutasiDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<MutasiDto>>([]);

        public Task<MutasiDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<MutasiDto?>(null);

        public Task<MutasiDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<MutasiDto?>(null);

        public Task<bool> MutasiNumberExistsAsync(string mutasiNumber, CancellationToken ct = default)
            => Task.FromResult(MutasiNumberExists);
    }

    private sealed class FakeStockRepository(Domain.Stock.Stock? stock) : IStockRepository
    {
        public Task<Domain.Stock.Stock?> GetByIdAsync(StockId id, CancellationToken ct = default)
            => Task.FromResult(stock);

        public Task SaveAsync(Domain.Stock.Stock s, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class FakeStockQueryRepository(Domain.Stock.Stock? stock) : IStockQueryRepository
    {
        public Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);

        public Task<StockDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<StockDto?>(null);

        public Task<StockDto?> GetByNoMesinAsync(string noMesin, CancellationToken ct = default)
        {
            if (stock is null) return Task.FromResult<StockDto?>(null);
            var dto = new StockDto(stock.Id.Value, stock.NoMesin, stock.NoRangka,
                stock.TipeMotorId, stock.WarnaId, stock.KiosId, stock.SuratJalanId,
                stock.SuratJalanDate, stock.SoId, stock.AssemblyYear, stock.Status, stock.IsActive);
            return Task.FromResult<StockDto?>(dto);
        }

        public Task<bool> NoMesinExistsAsync(string noMesin, CancellationToken ct = default)
            => Task.FromResult(stock is not null);

        public Task<IReadOnlyList<StockDto>> GetAvailableForKiosAsync(Guid kiosId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);

        public Task<IReadOnlyList<StockDto>> GetAllByTipeMotorAsync(Guid tipeMotorId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
