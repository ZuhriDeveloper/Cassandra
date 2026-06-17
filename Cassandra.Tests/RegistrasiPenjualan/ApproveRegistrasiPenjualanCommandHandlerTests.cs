using Cassandra.Application.Commands.RegistrasiPenjualan.ApproveRegistrasiPenjualan;
using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Application.Contracts.Kios;
using Cassandra.Application.Contracts.Mediator;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.DTOs.Karyawan;
using Cassandra.Application.DTOs.Kios;
using Cassandra.Application.DTOs.Mediator;
using Cassandra.Application.DTOs.RegistrasiPenjualan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;
using Cassandra.Domain.Kios;
using Cassandra.Domain.Mediator;
using Cassandra.Domain.RegistrasiPenjualan;

namespace Cassandra.Tests.RegistrasiPenjualan;

public class ApproveRegistrasiPenjualanCommandHandlerTests
{
    private static readonly Guid DealerId   = Guid.NewGuid();
    private static readonly Guid KaryawanId = Guid.NewGuid();
    private static readonly Guid KiosId     = Guid.NewGuid();

    private static Domain.RegistrasiPenjualan.RegistrasiPenjualan CreatePendingRegistrasi(
        decimal total = 20_000_000m)
        => Domain.RegistrasiPenjualan.RegistrasiPenjualan.Create(
            "PJ-001", DateOnly.FromDateTime(DateTime.Today),
            KaryawanId, KiosId, null,
            MetodePenjualanConstants.CASH, TipePenjualanConstants.DIRECT,
            "M001", "R001", "Customer A", "Addr", "0812", null, null,
            0m, 0m, 0m, 0m, 0m, total,
            0m, 0m, 0m, 0m, null, null,
            "CODE", "WARNA", "STK", null, "", false, "admin", DealerId);

    [Fact]
    public async Task HandleAsync_ApprovesRegistrasi()
    {
        var registrasi = CreatePendingRegistrasi();
        var repo = new FakeRegistrasiPenjualanRepository(registrasi);
        var handler = BuildHandler(repo);

        await handler.HandleAsync(
            new ApproveRegistrasiPenjualanCommand(registrasi.Id.Value, 1_000_000m, "manager"),
            TestContext.Current.CancellationToken);

        Assert.True(repo.Saved!.IsApproved);
        Assert.Equal(1_000_000m, repo.Saved.ApprovedDiscount);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenRegistrasiNotFound()
    {
        var repo    = new FakeRegistrasiPenjualanRepository(null);
        var handler = BuildHandler(repo);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(
                new ApproveRegistrasiPenjualanCommand(Guid.NewGuid(), 0m, "manager"),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenAlreadyApproved()
    {
        var registrasi = CreatePendingRegistrasi();
        registrasi.Approve(0m, "manager");
        registrasi.ClearDomainEvents();
        var repo    = new FakeRegistrasiPenjualanRepository(registrasi);
        var handler = BuildHandler(repo);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(
                new ApproveRegistrasiPenjualanCommand(registrasi.Id.Value, 0m, "manager"),
                TestContext.Current.CancellationToken));
    }

    private static ApproveRegistrasiPenjualanCommandHandler BuildHandler(
        FakeRegistrasiPenjualanRepository repo)
    {
        return new ApproveRegistrasiPenjualanCommandHandler(
            repo,
            new FakeRegistrasiPenjualanQueryRepo(),
            new FakeKaryawanRepository(KaryawanId),
            new FakeKaryawanQueryRepository(KaryawanId),
            new FakeKiosRepository(KiosId),
            new FakeKiosQueryRepository(KiosId),
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
            => Task.FromResult(_stored?.Id == id ? _stored : null);

        public Task SaveAsync(Domain.RegistrasiPenjualan.RegistrasiPenjualan registrasi, CancellationToken ct = default)
        {
            Saved = registrasi;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeRegistrasiPenjualanQueryRepo : IRegistrasiPenjualanQueryRepository
    {
        public Task<IReadOnlyList<RegistrasiPenjualanDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<RegistrasiPenjualanDto>>([]);
        public Task<RegistrasiPenjualanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<RegistrasiPenjualanDto?>(null);
        public Task<bool> NoPenjualanExistsAsync(string noPenjualan, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeKaryawanRepository : IKaryawanRepository
    {
        private readonly Guid _id;
        public FakeKaryawanRepository(Guid id) => _id = id;

        public Task<Domain.Karyawan.Karyawan?> GetByIdAsync(KaryawanId id, CancellationToken ct = default)
        {
            if (id.Value != _id) return Task.FromResult<Domain.Karyawan.Karyawan?>(null);
            var k = Domain.Karyawan.Karyawan.Create(
                "Test", "test@test.com", "1234567890123456",
                Domain.Karyawan.Gender.Male, DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
                "08123", null, "Addr", 100_000_000m, Guid.NewGuid(), "admin", DealerId);
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
                _id, "Test", "test@test.com", "123", "Male",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1)), null,
                "08123", null, "Addr", 100_000_000m, Guid.NewGuid(), true));
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
        private readonly Guid _id;
        public FakeKiosRepository(Guid id) => _id = id;

        public Task<Domain.Kios.Kios?> GetByIdAsync(KiosId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Kios.Kios?>(null);

        public Task SaveAsync(Domain.Kios.Kios kios, CancellationToken ct = default)
            => Task.CompletedTask;
    }

    private sealed class FakeKiosQueryRepository : IKiosQueryRepository
    {
        private readonly Guid _id;
        public FakeKiosQueryRepository(Guid id) => _id = id;

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
