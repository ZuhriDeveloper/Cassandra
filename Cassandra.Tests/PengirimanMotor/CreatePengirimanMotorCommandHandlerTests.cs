using Cassandra.Application.Commands.PengirimanMotor.CreatePengirimanMotor;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.PengirimanMotor;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.Contracts.Stock;
using Cassandra.Application.DTOs.PengirimanMotor;
using Cassandra.Application.DTOs.RegistrasiPenjualan;
using Cassandra.Application.DTOs.Stock;
using Cassandra.Domain.Common;
using Cassandra.Domain.PengirimanMotor;
using Cassandra.Domain.RegistrasiPenjualan;
using Cassandra.Domain.Stock;

namespace Cassandra.Tests.PengirimanMotor;

public class CreatePengirimanMotorCommandHandlerTests
{
    private static readonly Guid DealerId             = Guid.NewGuid();
    private static readonly Guid RegistrasiId         = Guid.NewGuid();
    private static readonly Guid StockRecordId        = Guid.NewGuid();
    private static readonly Guid KaryawanId           = Guid.NewGuid();
    private static readonly Guid KiosId               = Guid.NewGuid();
    private static readonly Guid TipeMotorId          = Guid.NewGuid();
    private static readonly Guid WarnaId              = Guid.NewGuid();

    private static CreatePengirimanMotorCommand DefaultCommand() =>
        new(RegistrasiId, "M001", Guid.NewGuid(), null,
            DateOnly.FromDateTime(DateTime.Today), "Zona A", "driver");

    private static RegistrasiPenjualanDto ApprovedRegistrasiDto() =>
        new(RegistrasiId, "PJ-001", DateOnly.FromDateTime(DateTime.Today),
            KaryawanId, KiosId, null, "CASH", "DIRECT",
            "M001", "R001", "Customer", "Addr", "0812", null, null,
            0m, 0m, 0m, 1_000_000m, 0m, 20_000_000m,
            0m, 0m, 0m, 0m, null, null,
            "CODE", "WARNA", "STK", null, [], true, false, false, false, "APPROVED");

    [Fact]
    public async Task HandleAsync_CreatesDelivery()
    {
        var repo       = new FakePengirimanMotorRepository();
        var stockDto   = new StockDto(StockRecordId, "M001", "R001", TipeMotorId, WarnaId,
            KiosId, "SJ-001", DateOnly.FromDateTime(DateTime.Today), Guid.NewGuid(), "2025",
            StockStatus.TERJUAL, true);
        var handler = BuildHandler(repo: repo, registrasiDto: ApprovedRegistrasiDto(), stockDto: stockDto);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenRegistrasiNotFound()
    {
        var handler = BuildHandler(registrasiDto: null);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenRegistrasiNotApproved()
    {
        var pendingDto = new RegistrasiPenjualanDto(RegistrasiId, "PJ-001",
            DateOnly.FromDateTime(DateTime.Today), KaryawanId, KiosId, null,
            "CASH", "DIRECT", "M001", "R001", "Customer", "Addr", "0812", null, null,
            0m, 0m, 0m, 0m, 0m, 20_000_000m, 0m, 0m, 0m, 0m, null, null,
            "CODE", "WARNA", "STK", null, [], false, false, false, false, "PENDING");

        var handler = BuildHandler(registrasiDto: pendingDto);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenRegistrasiAlreadySent()
    {
        var sentDto = new RegistrasiPenjualanDto(RegistrasiId, "PJ-001",
            DateOnly.FromDateTime(DateTime.Today), KaryawanId, KiosId, null,
            "CASH", "DIRECT", "M001", "R001", "Customer", "Addr", "0812", null, null,
            0m, 0m, 0m, 1_000_000m, 0m, 20_000_000m, 0m, 0m, 0m, 0m, null, null,
            "CODE", "WARNA", "STK", null, [], true, true, false, false, "SENT");

        var handler = BuildHandler(registrasiDto: sentDto);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenStockNotTerjual()
    {
        var stockDto = new StockDto(StockRecordId, "M001", "R001", TipeMotorId, WarnaId,
            KiosId, "SJ-001", DateOnly.FromDateTime(DateTime.Today), Guid.NewGuid(), "2025",
            StockStatus.TERSEDIA, true);

        var handler = BuildHandler(registrasiDto: ApprovedRegistrasiDto(), stockDto: stockDto);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_ChangesStockToTerkirim()
    {
        var stockDto  = new StockDto(StockRecordId, "M001", "R001", TipeMotorId, WarnaId,
            KiosId, "SJ-001", DateOnly.FromDateTime(DateTime.Today), Guid.NewGuid(), "2025",
            StockStatus.TERJUAL, true);
        var stockRepo = new FakeStockRepository(stockDto);
        var handler   = BuildHandler(registrasiDto: ApprovedRegistrasiDto(), stockDto: stockDto, stockRepo: stockRepo);

        await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.Equal(StockStatus.TERKIRIM, stockRepo.SavedStock?.Status);
    }

    [Fact]
    public async Task HandleAsync_MarksRegistrasiAsSent()
    {
        var stockDto      = new StockDto(StockRecordId, "M001", "R001", TipeMotorId, WarnaId,
            KiosId, "SJ-001", DateOnly.FromDateTime(DateTime.Today), Guid.NewGuid(), "2025",
            StockStatus.TERJUAL, true);
        var registrasiRepo = new FakeRegistrasiPenjualanRepository(
            Domain.RegistrasiPenjualan.RegistrasiPenjualan.Create(
                "PJ-001", DateOnly.FromDateTime(DateTime.Today),
                KaryawanId, KiosId, null, "CASH", "DIRECT",
                "M001", "R001", "Customer", "Addr", "0812", null, null,
                0m, 0m, 0m, 1_000_000m, 0m, 20_000_000m, 0m, 0m, 0m, 0m, null, null,
                "CODE", "WARNA", "STK", null, "", true, "admin", DealerId));

        var handler = BuildHandler(registrasiDto: ApprovedRegistrasiDto(), stockDto: stockDto,
            registrasiRepo: registrasiRepo);

        await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.True(registrasiRepo.Saved?.IsSent);
    }

    // ── Builder ───────────────────────────────────────────────────────────────

    private static CreatePengirimanMotorCommandHandler BuildHandler(
        FakePengirimanMotorRepository?    repo            = null,
        RegistrasiPenjualanDto?           registrasiDto   = null,
        StockDto?                         stockDto        = null,
        FakeRegistrasiPenjualanRepository? registrasiRepo = null,
        FakeStockRepository?              stockRepo       = null)
    {
        repo           ??= new FakePengirimanMotorRepository();
        registrasiRepo ??= registrasiDto is null
            ? new FakeRegistrasiPenjualanRepository(null)
            : new FakeRegistrasiPenjualanRepository(
                Domain.RegistrasiPenjualan.RegistrasiPenjualan.Create(
                    "PJ-001", DateOnly.FromDateTime(DateTime.Today),
                    KaryawanId, KiosId, null, "CASH", "DIRECT",
                    "M001", "R001", "Customer", "Addr", "0812", null, null,
                    0m, 0m, 0m, 1_000_000m, 0m, 20_000_000m, 0m, 0m, 0m, 0m, null, null,
                    "CODE", "WARNA", "STK", null, "", registrasiDto.IsApproved, "admin", DealerId));

        var effectiveStockDto = stockDto ?? new StockDto(StockRecordId, "M001", "R001", TipeMotorId, WarnaId,
            KiosId, "SJ-001", DateOnly.FromDateTime(DateTime.Today), Guid.NewGuid(), "2025",
            StockStatus.TERJUAL, true);

        stockRepo ??= new FakeStockRepository(effectiveStockDto);

        return new CreatePengirimanMotorCommandHandler(
            repo,
            registrasiRepo,
            new FakeRegistrasiPenjualanQueryRepo(registrasiDto),
            stockRepo,
            new FakeStockQueryRepository(effectiveStockDto),
            new FakeCurrentDealer(DealerId));
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakePengirimanMotorRepository : IPengirimanMotorRepository
    {
        public Domain.PengirimanMotor.PengirimanMotor? Saved { get; private set; }

        public Task<Domain.PengirimanMotor.PengirimanMotor?> GetByIdAsync(
            PengirimanMotorId id, CancellationToken ct = default)
            => Task.FromResult<Domain.PengirimanMotor.PengirimanMotor?>(null);

        public Task SaveAsync(Domain.PengirimanMotor.PengirimanMotor pengiriman, CancellationToken ct = default)
        {
            Saved = pengiriman;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRegistrasiPenjualanRepository : IRegistrasiPenjualanRepository
    {
        private readonly Domain.RegistrasiPenjualan.RegistrasiPenjualan? _stored;
        public Domain.RegistrasiPenjualan.RegistrasiPenjualan? Saved { get; private set; }

        public FakeRegistrasiPenjualanRepository(Domain.RegistrasiPenjualan.RegistrasiPenjualan? stored)
            => _stored = stored;

        public Task<Domain.RegistrasiPenjualan.RegistrasiPenjualan?> GetByIdAsync(
            RegistrasiPenjualanId id, CancellationToken ct = default)
            => Task.FromResult<Domain.RegistrasiPenjualan.RegistrasiPenjualan?>(_stored);

        public Task SaveAsync(Domain.RegistrasiPenjualan.RegistrasiPenjualan registrasi, CancellationToken ct = default)
        {
            Saved = registrasi;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRegistrasiPenjualanQueryRepo : IRegistrasiPenjualanQueryRepository
    {
        private readonly RegistrasiPenjualanDto? _dto;
        public FakeRegistrasiPenjualanQueryRepo(RegistrasiPenjualanDto? dto) => _dto = dto;

        public Task<IReadOnlyList<RegistrasiPenjualanDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<RegistrasiPenjualanDto>>([]);

        public Task<RegistrasiPenjualanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_dto?.Id == id ? _dto : null);

        public Task<bool> NoPenjualanExistsAsync(string noPenjualan, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeStockRepository : IStockRepository
    {
        private readonly StockDto? _dto;
        public Domain.Stock.Stock? SavedStock { get; private set; }

        public FakeStockRepository(StockDto? dto) => _dto = dto;

        public Task<Domain.Stock.Stock?> GetByIdAsync(StockId id, CancellationToken ct = default)
        {
            if (_dto is null) return Task.FromResult<Domain.Stock.Stock?>(null);
            var stock = Domain.Stock.Stock.Create(
                _dto.NoMesin, _dto.NoRangka, _dto.TipeMotorId, _dto.WarnaId,
                _dto.KiosId, _dto.SuratJalanId, _dto.SuratJalanDate, _dto.SoId,
                _dto.AssemblyYear, "admin", DealerId);
            stock.ClearDomainEvents();
            if (_dto.Status != StockStatus.TERSEDIA)
                stock.ChangeStatus(_dto.Status, "admin");
            stock.ClearDomainEvents();
            return Task.FromResult<Domain.Stock.Stock?>(stock);
        }

        public Task SaveAsync(Domain.Stock.Stock stock, CancellationToken ct = default)
        {
            SavedStock = stock;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeStockQueryRepository : IStockQueryRepository
    {
        private readonly StockDto? _dto;
        public FakeStockQueryRepository(StockDto? dto) => _dto = dto;

        public Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);

        public Task<StockDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(_dto?.Id == id ? _dto : null);

        public Task<StockDto?> GetByNoMesinAsync(string noMesin, CancellationToken ct = default)
            => Task.FromResult(_dto?.NoMesin == noMesin ? _dto : null);

        public Task<bool> NoMesinExistsAsync(string noMesin, CancellationToken ct = default)
            => Task.FromResult(_dto?.NoMesin == noMesin);

        public Task<IReadOnlyList<StockDto>> GetAvailableForKiosAsync(Guid kiosId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);

        public Task<IReadOnlyList<StockDto>> GetAllByTipeMotorAsync(Guid tipeMotorId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid  DealerId      => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool  IsSuperAdmin  => false;
    }
}
