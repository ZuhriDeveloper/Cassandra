using Cassandra.Application.Commands.RegistrasiPenjualan.VoidRegistrasiPenjualan;
using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Application.Contracts.Kios;
using Cassandra.Application.Contracts.Mediator;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.Contracts.Stock;
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

public class VoidRegistrasiPenjualanCommandHandlerTests
{
    private static readonly Guid DealerId   = Guid.NewGuid();
    private static readonly Guid KaryawanId = Guid.NewGuid();
    private static readonly Guid KiosId     = Guid.NewGuid();

    private static Domain.RegistrasiPenjualan.RegistrasiPenjualan CreateEnableToVoidRegistrasi()
    {
        var reg = Domain.RegistrasiPenjualan.RegistrasiPenjualan.Create(
            "PJ-001", DateOnly.FromDateTime(DateTime.Today),
            KaryawanId, KiosId, null,
            MetodePenjualanConstants.CASH, TipePenjualanConstants.DIRECT,
            "M001", "R001", "Customer A", "Addr", "0812", null, null,
            0m, 0m, 0m, 0m, 0m, 20_000_000m,
            0m, 0m, 0m, 0m, null, null,
            "CODE", "WARNA", "STK", null, "", false, "admin", DealerId);
        reg.SetEnableToVoid(true, "admin");
        reg.ClearDomainEvents();
        return reg;
    }

    [Fact]
    public async Task HandleAsync_VoidsRegistrasi()
    {
        var registrasi = CreateEnableToVoidRegistrasi();
        var repo       = new FakeRegistrasiPenjualanRepository(registrasi);
        var handler    = BuildHandler(repo);

        await handler.HandleAsync(
            new VoidRegistrasiPenjualanCommand(registrasi.Id.Value, "admin"),
            TestContext.Current.CancellationToken);

        Assert.True(repo.Saved!.IsVoid);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenRegistrasiNotFound()
    {
        var handler = BuildHandler(new FakeRegistrasiPenjualanRepository(null));

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(
                new VoidRegistrasiPenjualanCommand(Guid.NewGuid(), "admin"),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenNotEnableToVoid()
    {
        var reg = Domain.RegistrasiPenjualan.RegistrasiPenjualan.Create(
            "PJ-002", DateOnly.FromDateTime(DateTime.Today),
            KaryawanId, KiosId, null,
            MetodePenjualanConstants.CASH, TipePenjualanConstants.DIRECT,
            "M001", "R001", "Customer", "Addr", "0812", null, null,
            0m, 0m, 0m, 0m, 0m, 20_000_000m,
            0m, 0m, 0m, 0m, null, null,
            "CODE", "WARNA", "STK", null, "", false, "admin", DealerId);
        reg.ClearDomainEvents();
        var repo    = new FakeRegistrasiPenjualanRepository(reg);
        var handler = BuildHandler(repo);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(
                new VoidRegistrasiPenjualanCommand(reg.Id.Value, "admin"),
                TestContext.Current.CancellationToken));
    }

    private static VoidRegistrasiPenjualanCommandHandler BuildHandler(
        FakeRegistrasiPenjualanRepository repo)
    {
        return new VoidRegistrasiPenjualanCommandHandler(
            repo,
            new FakeStockRepository(),
            new FakeStockQueryRepository(),
            new FakeKaryawanRepository(KaryawanId),
            new FakeKaryawanQueryRepository(KaryawanId),
            new FakeKiosRepository(),
            new FakeKiosQueryRepository(),
            new FakeMediatorRepository(),
            new FakeMediatorQueryRepository());
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeRegistrasiPenjualanRepository : IRegistrasiPenjualanRepository
    {
        private readonly Domain.RegistrasiPenjualan.RegistrasiPenjualan? _stored;
        public Domain.RegistrasiPenjualan.RegistrasiPenjualan? Saved { get; private set; }

        public FakeRegistrasiPenjualanRepository(Domain.RegistrasiPenjualan.RegistrasiPenjualan? stored)
            => _stored = stored;

        public Task<Domain.RegistrasiPenjualan.RegistrasiPenjualan?> GetByIdAsync(
            RegistrasiPenjualanId id, CancellationToken ct = default)
            => Task.FromResult(_stored?.Id.Value == id.Value ? _stored : null);

        public Task SaveAsync(Domain.RegistrasiPenjualan.RegistrasiPenjualan registrasi, CancellationToken ct = default)
        {
            Saved = registrasi;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeStockRepository : IStockRepository
    {
        public Task<Domain.Stock.Stock?> GetByIdAsync(StockId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Stock.Stock?>(null);
        public Task SaveAsync(Domain.Stock.Stock stock, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class FakeStockQueryRepository : IStockQueryRepository
    {
        public Task<IReadOnlyList<StockDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);
        public Task<StockDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<StockDto?>(null);
        public Task<StockDto?> GetByNoMesinAsync(string noMesin, CancellationToken ct = default)
            => Task.FromResult<StockDto?>(null);
        public Task<bool> NoMesinExistsAsync(string noMesin, CancellationToken ct = default)
            => Task.FromResult(false);
        public Task<IReadOnlyList<StockDto>> GetAvailableForKiosAsync(Guid kiosId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);
        public Task<IReadOnlyList<StockDto>> GetAllByTipeMotorAsync(Guid tipeMotorId, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StockDto>>([]);
    }

    private sealed class FakeKaryawanRepository : IKaryawanRepository
    {
        private readonly Guid _id;
        public FakeKaryawanRepository(Guid id) => _id = id;
        public Task<Domain.Karyawan.Karyawan?> GetByIdAsync(KaryawanId id, CancellationToken ct = default)
        {
            if (id.Value != _id) return Task.FromResult<Domain.Karyawan.Karyawan?>(null);
            var k = Domain.Karyawan.Karyawan.Create("Test", "t@t.com", "1234567890123456",
                Domain.Karyawan.Gender.Male, DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
                "0812", null, "Addr", 100_000_000m, Guid.NewGuid(), "admin", DealerId);
            k.ClearDomainEvents();
            return Task.FromResult<Domain.Karyawan.Karyawan?>(k);
        }
        public Task SaveAsync(Domain.Karyawan.Karyawan karyawan, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class FakeKaryawanQueryRepository : IKaryawanQueryRepository
    {
        private readonly Guid _id;
        public FakeKaryawanQueryRepository(Guid id) => _id = id;
        public Task<KaryawanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            if (id != _id) return Task.FromResult<KaryawanDto?>(null);
            return Task.FromResult<KaryawanDto?>(new KaryawanDto(
                _id, "Test", "t@t.com", "123", "Male",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1)), null,
                "0812", null, "Addr", 100_000_000m, Guid.NewGuid(), true));
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
        public Task<Domain.Kios.Kios?> GetByIdAsync(KiosId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Kios.Kios?>(null);
        public Task SaveAsync(Domain.Kios.Kios kios, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class FakeKiosQueryRepository : IKiosQueryRepository
    {
        public Task<KiosDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<KiosDto?>(null);
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
}
