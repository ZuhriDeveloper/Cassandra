using Cassandra.Application.Commands.GrupTipeMotor.CreateGrupTipeMotor;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.GrupTipeMotor;
using Cassandra.Application.DTOs.GrupTipeMotor;
using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTipeMotor;

namespace Cassandra.Tests.GrupTipeMotor;

public class CreateGrupTipeMotorCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateGrupTipeMotorCommand DefaultCommand(string code = "SPORT") =>
        new(code, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeIsUnique()
    {
        var repo = new FakeGrupTipeMotorRepository();
        var query = new FakeGrupTipeMotorQueryRepository { CodeExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateGrupTipeMotorCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("SPORT", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeGrupTipeMotorRepository();
        var query = new FakeGrupTipeMotorQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateGrupTipeMotorCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeGrupTipeMotorRepository : IGrupTipeMotorRepository
    {
        public Domain.GrupTipeMotor.GrupTipeMotor? Saved { get; private set; }

        public Task<Domain.GrupTipeMotor.GrupTipeMotor?> GetByIdAsync(GrupTipeMotorId id, CancellationToken ct = default)
            => Task.FromResult<Domain.GrupTipeMotor.GrupTipeMotor?>(null);

        public Task SaveAsync(Domain.GrupTipeMotor.GrupTipeMotor grup, CancellationToken ct = default)
        {
            Saved = grup;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeGrupTipeMotorQueryRepository : IGrupTipeMotorQueryRepository
    {
        public bool CodeExists { get; init; }

        public Task<GrupTipeMotorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<GrupTipeMotorDto?>(null);

        public Task<IReadOnlyList<GrupTipeMotorDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<GrupTipeMotorDto>>([]);

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
