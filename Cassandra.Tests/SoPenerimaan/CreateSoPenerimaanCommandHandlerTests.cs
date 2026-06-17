using Cassandra.Application.Commands.SoPenerimaan.CreateSoPenerimaan;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.So;
using Cassandra.Application.Contracts.SoPenerimaan;
using Cassandra.Application.Contracts.Stock;
using Cassandra.Application.DTOs.So;
using Cassandra.Application.DTOs.SoPenerimaan;
using Cassandra.Application.DTOs.Stock;
using Cassandra.Domain.Common;
using Cassandra.Domain.SoPenerimaan;
using Cassandra.Domain.Stock;

namespace Cassandra.Tests.SoPenerimaan;

public class CreateSoPenerimaanCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid SoId = Guid.NewGuid();

    private static CreateSoPenerimaanCommand DefaultCommand(string suratJalanId = "SJ-001") =>
        new(
            suratJalanId,
            DateOnly.FromDateTime(DateTime.Today),
            SoId,
            [new CreateSoPenerimaanItemMotorRequest(Guid.NewGuid(), Guid.NewGuid(), "M001", "R001", Guid.NewGuid(), "2025")],
            [],
            "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenAllUnique()
    {
        var repo = new FakeSoPenerimaanRepository();
        var queryRepo = new FakeSoPenerimaanQueryRepository { SuratJalanIdExists = false };
        var stockRepo = new FakeStockRepository();
        var stockQueryRepo = new FakeStockQueryRepository { NoMesinExists = false };
        var soQueryRepo = new FakeSoQueryRepository { IsSoAktif = true };
        var dealer = new FakeCurrentDealer(DealerId);

        var handler = new CreateSoPenerimaanCommandHandler(
            repo, queryRepo, stockRepo, stockQueryRepo, soQueryRepo, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        // Stock should have been created for each motor item
        Assert.Single(stockRepo.Saved);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenSuratJalanIdExists()
    {
        var repo = new FakeSoPenerimaanRepository();
        var queryRepo = new FakeSoPenerimaanQueryRepository { SuratJalanIdExists = true };
        var stockRepo = new FakeStockRepository();
        var stockQueryRepo = new FakeStockQueryRepository { NoMesinExists = false };
        var soQueryRepo = new FakeSoQueryRepository { IsSoAktif = true };
        var dealer = new FakeCurrentDealer(DealerId);

        var handler = new CreateSoPenerimaanCommandHandler(
            repo, queryRepo, stockRepo, stockQueryRepo, soQueryRepo, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenNoMesinAlreadyExists()
    {
        var repo = new FakeSoPenerimaanRepository();
        var queryRepo = new FakeSoPenerimaanQueryRepository { SuratJalanIdExists = false };
        var stockRepo = new FakeStockRepository();
        var stockQueryRepo = new FakeStockQueryRepository { NoMesinExists = true };
        var soQueryRepo = new FakeSoQueryRepository { IsSoAktif = true };
        var dealer = new FakeCurrentDealer(DealerId);

        var handler = new CreateSoPenerimaanCommandHandler(
            repo, queryRepo, stockRepo, stockQueryRepo, soQueryRepo, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenSoNotAktif()
    {
        var repo = new FakeSoPenerimaanRepository();
        var queryRepo = new FakeSoPenerimaanQueryRepository { SuratJalanIdExists = false };
        var stockRepo = new FakeStockRepository();
        var stockQueryRepo = new FakeStockQueryRepository { NoMesinExists = false };
        var soQueryRepo = new FakeSoQueryRepository { IsSoAktif = false };
        var dealer = new FakeCurrentDealer(DealerId);

        var handler = new CreateSoPenerimaanCommandHandler(
            repo, queryRepo, stockRepo, stockQueryRepo, soQueryRepo, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    // ── Fakes ──────────────────────────────────────────────────────────────────

    private sealed class FakeSoPenerimaanRepository : ISoPenerimaanRepository
    {
        public Domain.SoPenerimaan.SoPenerimaan? Saved { get; private set; }

        public Task<Domain.SoPenerimaan.SoPenerimaan?> GetByIdAsync(SoPenerimaanId id, CancellationToken ct = default)
            => Task.FromResult<Domain.SoPenerimaan.SoPenerimaan?>(null);

        public Task SaveAsync(Domain.SoPenerimaan.SoPenerimaan sp, CancellationToken ct = default)
        {
            Saved = sp;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeSoPenerimaanQueryRepository : ISoPenerimaanQueryRepository
    {
        public bool SuratJalanIdExists { get; init; }

        public Task<IReadOnlyList<SoPenerimaanDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<SoPenerimaanDto>>([]);

        public Task<SoPenerimaanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<SoPenerimaanDto?>(null);

        public Task<SoPenerimaanDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<SoPenerimaanDto?>(null);

        public Task<bool> SuratJalanIdExistsAsync(string suratJalanId, CancellationToken ct = default)
            => Task.FromResult(SuratJalanIdExists);

        public Task<bool> HasPenerimaanForSoAsync(Guid soId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeStockRepository : IStockRepository
    {
        public List<Domain.Stock.Stock> Saved { get; } = [];

        public Task<Domain.Stock.Stock?> GetByIdAsync(StockId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Stock.Stock?>(null);

        public Task SaveAsync(Domain.Stock.Stock stock, CancellationToken ct = default)
        {
            Saved.Add(stock);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeStockQueryRepository : IStockQueryRepository
    {
        public bool NoMesinExists { get; init; }

        public Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);

        public Task<StockDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<StockDto?>(null);

        public Task<StockDto?> GetByNoMesinAsync(string noMesin, CancellationToken ct = default)
            => Task.FromResult<StockDto?>(null);

        public Task<bool> NoMesinExistsAsync(string noMesin, CancellationToken ct = default)
            => Task.FromResult(NoMesinExists);

        public Task<IReadOnlyList<StockDto>> GetAvailableForKiosAsync(Guid kiosId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);

        public Task<IReadOnlyList<StockDto>> GetAllByTipeMotorAsync(Guid tipeMotorId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);
    }

    private sealed class FakeSoQueryRepository : ISoQueryRepository
    {
        public bool IsSoAktif { get; init; }

        public Task<IReadOnlyList<SoDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<SoDto>>([]);

        public Task<SoDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<SoDto?>(null);

        public Task<SoDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<SoDto?>(null);

        public Task<bool> SoNumberExistsAsync(string soNumber, CancellationToken ct = default)
            => Task.FromResult(false);

        public Task<bool> IsSoAktifAsync(Guid soId, CancellationToken ct = default)
            => Task.FromResult(IsSoAktif);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
