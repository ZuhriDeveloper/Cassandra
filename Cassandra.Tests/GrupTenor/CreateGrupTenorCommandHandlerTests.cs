using Cassandra.Application.Commands.GrupTenor.CreateGrupTenor;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.GrupTenor;
using Cassandra.Application.DTOs.GrupTenor;
using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTenor;

namespace Cassandra.Tests.GrupTenor;

public class CreateGrupTenorCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateGrupTenorCommand DefaultCommand(string code = "GT01") =>
        new(code, "Group Tenor 1", "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeIsUnique()
    {
        var repo = new FakeGrupTenorRepository();
        var query = new FakeGrupTenorQueryRepository { CodeExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateGrupTenorCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("GT01", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeGrupTenorRepository();
        var query = new FakeGrupTenorQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateGrupTenorCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeGrupTenorRepository : IGrupTenorRepository
    {
        public Domain.GrupTenor.GrupTenor? Saved { get; private set; }

        public Task<Domain.GrupTenor.GrupTenor?> GetByIdAsync(GrupTenorId id, CancellationToken ct = default)
            => Task.FromResult<Domain.GrupTenor.GrupTenor?>(null);

        public Task SaveAsync(Domain.GrupTenor.GrupTenor grupTenor, CancellationToken ct = default)
        {
            Saved = grupTenor;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeGrupTenorQueryRepository : IGrupTenorQueryRepository
    {
        public bool CodeExists { get; init; }

        public Task<GrupTenorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<GrupTenorDto?>(null);

        public Task<IReadOnlyList<GrupTenorDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<GrupTenorDto>>([]);

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => Task.FromResult(CodeExists);

        public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
            => Task.FromResult(false);

        public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
