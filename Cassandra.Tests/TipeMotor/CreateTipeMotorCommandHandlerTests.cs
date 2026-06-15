using Cassandra.Application.Commands.TipeMotor.CreateTipeMotor;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.TipeMotor;
using Cassandra.Application.DTOs.TipeMotor;
using Cassandra.Domain.Common;
using Cassandra.Domain.TipeMotor;

namespace Cassandra.Tests.TipeMotor;

public class CreateTipeMotorCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid GrupId = Guid.NewGuid();

    private static CreateTipeMotorCommand DefaultCommand(string code = "CB150") =>
        new(code, GrupId, "CB150R", "PC001", "WMS001", "AHM001",
            "NXXX", "CXXX", 50_000_000m, 1_000_000m, 900_000m, 500_000m, 450_000m, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeIsUnique()
    {
        var repo = new FakeTipeMotorRepository();
        var query = new FakeTipeMotorQueryRepository { CodeExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateTipeMotorCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("CB150", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeTipeMotorRepository();
        var query = new FakeTipeMotorQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateTipeMotorCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeTipeMotorRepository : ITipeMotorRepository
    {
        public Domain.TipeMotor.TipeMotor? Saved { get; private set; }

        public Task<Domain.TipeMotor.TipeMotor?> GetByIdAsync(TipeMotorId id, CancellationToken ct = default)
            => Task.FromResult<Domain.TipeMotor.TipeMotor?>(null);

        public Task SaveAsync(Domain.TipeMotor.TipeMotor tipe, CancellationToken ct = default)
        {
            Saved = tipe;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeTipeMotorQueryRepository : ITipeMotorQueryRepository
    {
        public bool CodeExists { get; init; }

        public Task<TipeMotorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<TipeMotorDto?>(null);

        public Task<IReadOnlyList<TipeMotorDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<TipeMotorDto>>([]);

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => Task.FromResult(CodeExists);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
