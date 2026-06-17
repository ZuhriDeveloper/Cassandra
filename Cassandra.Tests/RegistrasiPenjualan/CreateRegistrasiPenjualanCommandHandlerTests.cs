using Cassandra.Application.Commands.RegistrasiPenjualan.CreateRegistrasiPenjualan;
using Cassandra.Application.Contracts.ArTransaction;
using Cassandra.Domain.ArTransaction;
using Cassandra.Application.Contracts.AlokasiDiskon;
using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Discount;
using Cassandra.Application.Contracts.DiscountCash;
using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Application.Contracts.Kios;
using Cassandra.Application.Contracts.Mediator;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.Contracts.Stock;
using Cassandra.Application.DTOs.AlokasiDiskon;
using Cassandra.Application.DTOs.DaftarHargaLeasing;
using Cassandra.Application.DTOs.Discount;
using Cassandra.Application.DTOs.DiscountCash;
using Cassandra.Application.DTOs.Karyawan;
using Cassandra.Application.DTOs.Kios;
using Cassandra.Application.DTOs.Mediator;
using Cassandra.Application.DTOs.RegistrasiPenjualan;
using Cassandra.Application.DTOs.Stock;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;
using Cassandra.Domain.Kios;
using Cassandra.Domain.Mediator;
using Cassandra.Domain.RegistrasiPenjualan;
using Cassandra.Domain.Stock;

namespace Cassandra.Tests.RegistrasiPenjualan;

public class CreateRegistrasiPenjualanCommandHandlerTests
{
    private static readonly Guid DealerId    = Guid.NewGuid();
    private static readonly Guid KaryawanId  = Guid.NewGuid();
    private static readonly Guid KiosId      = Guid.NewGuid();
    private static readonly Guid TipeMotorId = Guid.NewGuid();
    private static readonly Guid WarnaId     = Guid.NewGuid();
    private static readonly Guid StockId     = Guid.NewGuid();

    private static CreateRegistrasiPenjualanCommand DefaultCashDirectCommand(decimal discount = 1_000_000m) =>
        new(
            "PJ-001",
            DateOnly.FromDateTime(DateTime.Today),
            KaryawanId,
            KiosId,
            null,
            MetodePenjualanConstants.CASH,
            TipePenjualanConstants.DIRECT,
            "M001",
            "R001",
            "Customer A",
            "Jl. Merdeka",
            "08123456789",
            null, null,
            500_000m, 300_000m,
            discount,
            20_000_000m,
            0m, 0m, 0m,
            null, null,
            "STK-001", null,
            new List<string> { "Helm", "Kunci" },
            "admin");

    private CreateRegistrasiPenjualanCommandHandler BuildHandler(
        FakeRegistrasiPenjualanRepository?   registrasiRepo  = null,
        FakeRegistrasiPenjualanQueryRepo?    registrasiQuery = null,
        FakeStockRepository?                 stockRepo       = null,
        FakeStockQueryRepository?            stockQuery      = null,
        FakeKaryawanRepository?              karyawanRepo    = null,
        FakeKaryawanQueryRepository?         karyawanQuery   = null,
        FakeKiosRepository?                  kiosRepo        = null,
        FakeKiosQueryRepository?             kiosQuery       = null,
        FakeMediatorRepository?              mediatorRepo    = null,
        FakeMediatorQueryRepository?         mediatorQuery   = null,
        FakeDiscountCashQueryRepository?     dcQuery         = null,
        FakeDaftarHargaLeasingQueryRepo?     dhlQuery        = null,
        FakeDiscountQueryRepository?         discountQuery   = null,
        FakeAlokasiDiskonQueryRepository?    alokasiQuery    = null,
        decimal karyawanLimit = 100_000_000m,
        decimal kiosLimit     = 100_000_000m,
        string stockStatus    = StockStatus.TERSEDIA,
        decimal directDiscount = 1_000_000m)
    {
        registrasiRepo  ??= new FakeRegistrasiPenjualanRepository();
        registrasiQuery ??= new FakeRegistrasiPenjualanQueryRepo { NoPenjualanExists = false };

        var stockDto = new StockDto(StockId, "M001", "R001", TipeMotorId, WarnaId,
            KiosId, "SJ-001", DateOnly.FromDateTime(DateTime.Today), Guid.NewGuid(), "2025",
            stockStatus, true);

        stockRepo  ??= new FakeStockRepository(stockDto);
        stockQuery ??= new FakeStockQueryRepository(stockDto);

        karyawanRepo  ??= new FakeKaryawanRepository(KaryawanId, karyawanLimit);
        karyawanQuery ??= new FakeKaryawanQueryRepository(KaryawanId, karyawanLimit);

        kiosRepo  ??= new FakeKiosRepository(KiosId, kiosLimit);
        kiosQuery ??= new FakeKiosQueryRepository(KiosId, kiosLimit);

        mediatorRepo  ??= new FakeMediatorRepository();
        mediatorQuery ??= new FakeMediatorQueryRepository();

        dcQuery ??= new FakeDiscountCashQueryRepository(TipeMotorId, directDiscount, directDiscount);

        dhlQuery      ??= new FakeDaftarHargaLeasingQueryRepo();
        discountQuery ??= new FakeDiscountQueryRepository();
        alokasiQuery  ??= new FakeAlokasiDiskonQueryRepository(KaryawanId, "Direct");

        return new CreateRegistrasiPenjualanCommandHandler(
            registrasiRepo,
            registrasiQuery,
            stockRepo,
            stockQuery,
            karyawanRepo,
            karyawanQuery,
            kiosRepo,
            kiosQuery,
            mediatorRepo,
            mediatorQuery,
            dcQuery,
            dhlQuery,
            discountQuery,
            alokasiQuery,
            new FakeArTransactionRepository(),
            new FakeCurrentDealer(DealerId));
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public async Task HandleAsync_CashDirect_CreatesRegistrasi()
    {
        var repo = new FakeRegistrasiPenjualanRepository();
        var handler = BuildHandler(registrasiRepo: repo);

        var id = await handler.HandleAsync(DefaultCashDirectCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("PJ-001", repo.Saved!.NoPenjualan);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_CashDirect_AutoApproved_WhenDiscountEqualsStandard()
    {
        var repo    = new FakeRegistrasiPenjualanRepository();
        var handler = BuildHandler(registrasiRepo: repo, directDiscount: 1_000_000m);

        // discount == directDiscount → auto-approved
        await handler.HandleAsync(DefaultCashDirectCommand(discount: 1_000_000m), TestContext.Current.CancellationToken);

        Assert.True(repo.Saved!.IsApproved);
    }

    [Fact]
    public async Task HandleAsync_CashDirect_NotAutoApproved_WhenDiscountDiffers()
    {
        var repo    = new FakeRegistrasiPenjualanRepository();
        var handler = BuildHandler(registrasiRepo: repo, directDiscount: 1_000_000m);

        // discount != directDiscount → NOT auto-approved
        await handler.HandleAsync(DefaultCashDirectCommand(discount: 500_000m), TestContext.Current.CancellationToken);

        Assert.False(repo.Saved!.IsApproved);
    }

    [Fact]
    public async Task HandleAsync_ChangesStockToTerjual()
    {
        var stockRepo = new FakeStockRepository(
            new StockDto(StockId, "M001", "R001", TipeMotorId, WarnaId,
                KiosId, "SJ-001", DateOnly.FromDateTime(DateTime.Today), Guid.NewGuid(), "2025",
                StockStatus.TERSEDIA, true));
        var handler = BuildHandler(stockRepo: stockRepo);

        await handler.HandleAsync(DefaultCashDirectCommand(), TestContext.Current.CancellationToken);

        Assert.Equal(StockStatus.TERJUAL, stockRepo.SavedStock?.Status);
    }

    // ── Error cases ───────────────────────────────────────────────────────────

    [Fact]
    public async Task HandleAsync_Throws_WhenNoPenjualanExists()
    {
        var query = new FakeRegistrasiPenjualanQueryRepo { NoPenjualanExists = true };
        var handler = BuildHandler(registrasiQuery: query);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCashDirectCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenStockNotFound()
    {
        var stockQuery = new FakeStockQueryRepository(null);
        var handler = BuildHandler(stockQuery: stockQuery);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCashDirectCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenStockNotTersedia()
    {
        var stockDto = new StockDto(StockId, "M001", "R001", TipeMotorId, WarnaId,
            KiosId, "SJ-001", DateOnly.FromDateTime(DateTime.Today), Guid.NewGuid(), "2025",
            StockStatus.TERJUAL, true);
        var stockQuery = new FakeStockQueryRepository(stockDto);
        var stockRepo  = new FakeStockRepository(stockDto);
        var handler = BuildHandler(stockRepo: stockRepo, stockQuery: stockQuery);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCashDirectCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenKaryawanLimitInsufficient()
    {
        var handler = BuildHandler(karyawanLimit: 5_000_000m); // Total=20M, limit=5M → insufficient

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCashDirectCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenKiosLimitInsufficient()
    {
        var command = DefaultCashDirectCommand() with { TipePenjualan = TipePenjualanConstants.KIOS };
        var handler = BuildHandler(kiosLimit: 1_000m); // insufficient

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(command, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_DeductsKaryawanLimit()
    {
        var karyawanRepo = new FakeKaryawanRepository(KaryawanId, 100_000_000m);
        var handler = BuildHandler(karyawanRepo: karyawanRepo);

        await handler.HandleAsync(DefaultCashDirectCommand(), TestContext.Current.CancellationToken);

        // CASH: limitToDeduct = Total = 20M → new limit = 100M - 20M = 80M
        Assert.Equal(80_000_000m, karyawanRepo.SavedLimit);
    }

    // ── Kelengkapan stored as comma-joined string ─────────────────────────────

    [Fact]
    public async Task HandleAsync_StoresKelengkapanAsJoinedString()
    {
        var repo    = new FakeRegistrasiPenjualanRepository();
        var handler = BuildHandler(registrasiRepo: repo);

        await handler.HandleAsync(DefaultCashDirectCommand(), TestContext.Current.CancellationToken);

        Assert.Equal("Helm,Kunci", repo.Saved!.Kelengkapan);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeRegistrasiPenjualanRepository : IRegistrasiPenjualanRepository
    {
        public Domain.RegistrasiPenjualan.RegistrasiPenjualan? Saved { get; private set; }

        public Task<Domain.RegistrasiPenjualan.RegistrasiPenjualan?> GetByIdAsync(
            RegistrasiPenjualanId id, CancellationToken ct = default)
            => Task.FromResult<Domain.RegistrasiPenjualan.RegistrasiPenjualan?>(null);

        public Task SaveAsync(Domain.RegistrasiPenjualan.RegistrasiPenjualan registrasi, CancellationToken ct = default)
        {
            Saved = registrasi;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRegistrasiPenjualanQueryRepo : IRegistrasiPenjualanQueryRepository
    {
        public bool NoPenjualanExists { get; init; }

        public Task<IReadOnlyList<RegistrasiPenjualanDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<RegistrasiPenjualanDto>>([]);

        public Task<RegistrasiPenjualanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<RegistrasiPenjualanDto?>(null);

        public Task<bool> NoPenjualanExistsAsync(string noPenjualan, CancellationToken ct = default)
            => Task.FromResult(NoPenjualanExists);
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
            => Task.FromResult<IReadOnlyList<StockDto>>(_dto is null ? [] : [_dto]);

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

    private sealed class FakeKaryawanRepository : IKaryawanRepository
    {
        private readonly Guid    _id;
        private readonly decimal _limit;
        public decimal SavedLimit { get; private set; }

        public FakeKaryawanRepository(Guid id, decimal limit) { _id = id; _limit = limit; }

        public Task<Domain.Karyawan.Karyawan?> GetByIdAsync(KaryawanId id, CancellationToken ct = default)
        {
            if (id.Value != _id) return Task.FromResult<Domain.Karyawan.Karyawan?>(null);
            var k = Domain.Karyawan.Karyawan.Create(
                "Test Karyawan", "test@test.com", "1234567890123456",
                Domain.Karyawan.Gender.Male, DateOnly.FromDateTime(DateTime.Today.AddYears(-2)),
                "08123", null, "Addr", _limit, Guid.NewGuid(), "admin", DealerId);
            k.ClearDomainEvents();
            return Task.FromResult<Domain.Karyawan.Karyawan?>(k);
        }

        public Task SaveAsync(Domain.Karyawan.Karyawan karyawan, CancellationToken ct = default)
        {
            SavedLimit = karyawan.SalesLimit;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeKaryawanQueryRepository : IKaryawanQueryRepository
    {
        private readonly Guid    _id;
        private readonly decimal _limit;

        public FakeKaryawanQueryRepository(Guid id, decimal limit) { _id = id; _limit = limit; }

        public Task<KaryawanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            if (id != _id) return Task.FromResult<KaryawanDto?>(null);
            return Task.FromResult<KaryawanDto?>(new KaryawanDto(
                _id, "Test Karyawan", "test@test.com", "123",
                "Male", DateOnly.FromDateTime(DateTime.Today.AddYears(-2)), null,
                "08123", null, "Addr", _limit, Guid.NewGuid(), true));
        }

        public Task<IReadOnlyList<KaryawanDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<KaryawanDto>>([]);

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
            => Task.FromResult(false);

        public Task<bool> EmailExistsExcludingAsync(string email, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeKiosRepository : IKiosRepository
    {
        private readonly Guid    _id;
        private readonly decimal _limit;

        public FakeKiosRepository(Guid id, decimal limit) { _id = id; _limit = limit; }

        public Task<Domain.Kios.Kios?> GetByIdAsync(KiosId id, CancellationToken ct = default)
        {
            if (id.Value != _id) return Task.FromResult<Domain.Kios.Kios?>(null);
            var k = Domain.Kios.Kios.Create(
                "K001", "Kios Test", "08123", null, "Addr",
                Guid.NewGuid(), _limit, "admin", DealerId);
            k.ClearDomainEvents();
            return Task.FromResult<Domain.Kios.Kios?>(k);
        }

        public Task SaveAsync(Domain.Kios.Kios kios, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class FakeKiosQueryRepository : IKiosQueryRepository
    {
        private readonly Guid    _id;
        private readonly decimal _limit;

        public FakeKiosQueryRepository(Guid id, decimal limit) { _id = id; _limit = limit; }

        public Task<KiosDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            if (id != _id) return Task.FromResult<KiosDto?>(null);
            return Task.FromResult<KiosDto?>(new KiosDto(_id, "K001", "Kios Test", "08123", null, "Addr", Guid.NewGuid(), _limit, true));
        }

        public Task<IReadOnlyList<KiosDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<KiosDto>>([]);

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeMediatorRepository : IMediatorRepository
    {
        public Task<Domain.Mediator.Mediator?> GetByIdAsync(MediatorId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Mediator.Mediator?>(null);

        public Task SaveAsync(Domain.Mediator.Mediator mediator, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class FakeMediatorQueryRepository : IMediatorQueryRepository
    {
        public Task<MediatorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<MediatorDto?>(null);

        public Task<IReadOnlyList<MediatorDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<MediatorDto>>([]);

        public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
            => Task.FromResult(false);

        public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeDiscountCashQueryRepository : IDiscountCashQueryRepository
    {
        private readonly Guid    _tipeMotorId;
        private readonly decimal _direct;
        private readonly decimal _channel;

        public FakeDiscountCashQueryRepository(Guid tipeMotorId, decimal direct, decimal channel)
        { _tipeMotorId = tipeMotorId; _direct = direct; _channel = channel; }

        public Task<DiscountCashDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<DiscountCashDto?>(null);

        public Task<IReadOnlyList<DiscountCashDto>> GetAllAsync(CancellationToken ct = default)
        {
            IReadOnlyList<DiscountCashDto> list = [new DiscountCashDto(Guid.NewGuid(), _tipeMotorId, _direct, _channel, true)];
            return Task.FromResult(list);
        }

        public Task<bool> TipeMotorIdExistsAsync(Guid tipeMotorId, CancellationToken ct = default)
            => Task.FromResult(tipeMotorId == _tipeMotorId);
    }

    private sealed class FakeDaftarHargaLeasingQueryRepo : IDaftarHargaLeasingQueryRepository
    {
        public Task<DaftarHargaLeasingDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<DaftarHargaLeasingDto?>(new DaftarHargaLeasingDto(id, "DHL-001", Guid.NewGuid(), Guid.NewGuid(), true, []));

        public Task<IReadOnlyList<DaftarHargaLeasingDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<DaftarHargaLeasingDto>>([]);

        public Task<bool> ExistsAsync(string name, Guid globalLeasingId, Guid grupTenorId, CancellationToken ct = default)
            => Task.FromResult(false);

        public Task<bool> ExistsExcludingAsync(string name, Guid globalLeasingId, Guid grupTenorId, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeDiscountQueryRepository : IDiscountQueryRepository
    {
        public Task<DiscountDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<DiscountDto?>(null);

        public Task<IReadOnlyList<DiscountDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<DiscountDto>>([]);

        public Task<bool> ExistsAsync(Guid daftarHargaLeasingId, string level, CancellationToken ct = default)
            => Task.FromResult(false);

        public Task<bool> ExistsExcludingAsync(Guid daftarHargaLeasingId, string level, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeAlokasiDiskonQueryRepository : IAlokasiDiskonQueryRepository
    {
        private readonly Guid   _karyawanId;
        private readonly string _level;

        public FakeAlokasiDiskonQueryRepository(Guid karyawanId, string level)
        { _karyawanId = karyawanId; _level = level; }

        public Task<AlokasiDiskonDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<AlokasiDiskonDto?>(null);

        public Task<IReadOnlyList<AlokasiDiskonDto>> GetAllAsync(CancellationToken ct = default)
        {
            IReadOnlyList<AlokasiDiskonDto> list = [new AlokasiDiskonDto(Guid.NewGuid(), _karyawanId, _level, true)];
            return Task.FromResult(list);
        }

        public Task<bool> KaryawanIdExistsAsync(Guid karyawanId, CancellationToken ct = default)
            => Task.FromResult(karyawanId == _karyawanId);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid  DealerId      => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool  IsSuperAdmin  => false;
    }

    private sealed class FakeArTransactionRepository : IArTransactionRepository
    {
        public Task<Domain.ArTransaction.ArTransaction?> GetByIdAsync(ArTransactionId id, CancellationToken ct = default)
            => Task.FromResult<Domain.ArTransaction.ArTransaction?>(null);

        public Task SaveAsync(Domain.ArTransaction.ArTransaction arTransaction, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
