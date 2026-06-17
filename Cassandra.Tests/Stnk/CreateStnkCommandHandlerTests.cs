using Cassandra.Application.Commands.Stnk.CreateStnk;
using Cassandra.Application.Contracts.Bpkb;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Application.Contracts.Stnk;
using Cassandra.Application.DTOs.Bpkb;
using Cassandra.Application.DTOs.RegistrasiPenjualan;
using Cassandra.Application.DTOs.Stnk;
using Cassandra.Domain.Bpkb;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;

namespace Cassandra.Tests.Stnk;

public class CreateStnkCommandHandlerTests
{
    private static readonly Guid DealerId              = Guid.NewGuid();
    private static readonly Guid RegistrasiPenjualanId = Guid.NewGuid();
    private static readonly Guid KaryawanId            = Guid.NewGuid();
    private static readonly Guid KiosId                = Guid.NewGuid();

    private static CreateStnkCommand DefaultCommand() =>
        new(RegistrasiPenjualanId,
            DateOnly.FromDateTime(DateTime.Today),
            "Budi Santoso",
            "Jl. Merdeka No. 1",
            "admin");

    private static RegistrasiPenjualanDto SentRegistrasiDto() =>
        new(RegistrasiPenjualanId, "PJ-001", DateOnly.FromDateTime(DateTime.Today),
            KaryawanId, KiosId, null, "CASH", "DIRECT",
            "M001", "R001", "Budi", "Addr", "0812", null, null,
            0m, 0m, 0m, 0m, 0m, 20_000_000m,
            0m, 0m, 0m, 0m, null, null,
            "CODE", "WARNA", "STK", null, [], true, true, false, false, "SENT");

    [Fact]
    public async Task HandleAsync_CreatesStnkAndBpkb_HappyPath()
    {
        var stnkRepo = new FakeStnkRepository();
        var bpkbRepo = new FakeBpkbRepository();
        var handler  = BuildHandler(stnkRepo: stnkRepo, bpkbRepo: bpkbRepo,
                                    registrasiDto: SentRegistrasiDto(), stnkExists: false);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(stnkRepo.Saved);
        Assert.NotNull(bpkbRepo.Saved);
        Assert.Equal(RegistrasiPenjualanId, bpkbRepo.Saved!.RegistrasiPenjualanId);
        Assert.Equal(stnkRepo.Saved!.Id.Value, bpkbRepo.Saved.StnkId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenRegistrasiNotFound()
    {
        var handler = BuildHandler(registrasiDto: null);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenRegistrasiNotSent()
    {
        var notSentDto = new RegistrasiPenjualanDto(RegistrasiPenjualanId, "PJ-001",
            DateOnly.FromDateTime(DateTime.Today), KaryawanId, KiosId, null,
            "CASH", "DIRECT", "M001", "R001", "Budi", "Addr", "0812", null, null,
            0m, 0m, 0m, 0m, 0m, 20_000_000m, 0m, 0m, 0m, 0m, null, null,
            "CODE", "WARNA", "STK", null, [], true, false, false, false, "APPROVED");

        var handler = BuildHandler(registrasiDto: notSentDto, stnkExists: false);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenStnkAlreadyExists()
    {
        var handler = BuildHandler(registrasiDto: SentRegistrasiDto(), stnkExists: true);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));
    }

    // ── Builder ───────────────────────────────────────────────────────────────

    private static CreateStnkCommandHandler BuildHandler(
        FakeStnkRepository?          stnkRepo      = null,
        FakeBpkbRepository?          bpkbRepo      = null,
        RegistrasiPenjualanDto?      registrasiDto = null,
        bool                         stnkExists    = false)
    {
        stnkRepo ??= new FakeStnkRepository();
        bpkbRepo ??= new FakeBpkbRepository();

        return new CreateStnkCommandHandler(
            stnkRepo,
            new FakeStnkQueryRepository(stnkExists),
            bpkbRepo,
            new FakeRegistrasiPenjualanQueryRepo(registrasiDto),
            new FakeCurrentDealer(DealerId));
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeStnkRepository : IStnkRepository
    {
        public Domain.Stnk.Stnk? Saved { get; private set; }

        public Task<Domain.Stnk.Stnk?> GetByIdAsync(StnkId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Stnk.Stnk?>(null);

        public Task SaveAsync(Domain.Stnk.Stnk stnk, CancellationToken ct = default)
        {
            Saved = stnk;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeBpkbRepository : IBpkbRepository
    {
        public Domain.Bpkb.Bpkb? Saved { get; private set; }

        public Task<Domain.Bpkb.Bpkb?> GetByIdAsync(BpkbId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Bpkb.Bpkb?>(null);

        public Task SaveAsync(Domain.Bpkb.Bpkb bpkb, CancellationToken ct = default)
        {
            Saved = bpkb;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeStnkQueryRepository(bool exists) : IStnkQueryRepository
    {
        public Task<IReadOnlyList<StnkDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<StnkDto>>([]);

        public Task<StnkDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<StnkDto?>(null);

        public Task<bool> ExistsByRegistrasiPenjualanIdAsync(Guid registrasiPenjualanId, CancellationToken ct = default)
            => Task.FromResult(exists);
    }

    private sealed class FakeRegistrasiPenjualanQueryRepo(RegistrasiPenjualanDto? dto) : IRegistrasiPenjualanQueryRepository
    {
        public Task<IReadOnlyList<RegistrasiPenjualanDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<RegistrasiPenjualanDto>>([]);

        public Task<RegistrasiPenjualanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult(dto?.Id == id ? dto : null);

        public Task<bool> NoPenjualanExistsAsync(string noPenjualan, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid  DealerId       => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool  IsSuperAdmin   => false;
    }
}
