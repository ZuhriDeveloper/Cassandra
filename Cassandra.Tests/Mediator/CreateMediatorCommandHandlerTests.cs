using Cassandra.Application.Commands.Mediator.CreateMediator;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Mediator;
using Cassandra.Application.DTOs.Mediator;
using Cassandra.Domain.Common;
using Cassandra.Domain.Mediator;

namespace Cassandra.Tests.Mediator;

public class CreateMediatorCommandHandlerTests
{
    private static readonly Guid DealerId   = Guid.NewGuid();
    private static readonly Guid KaryawanId = Guid.NewGuid();

    private static CreateMediatorCommand DefaultCommand(string name = "Agen Utama") =>
        new(name, KaryawanId, "Jl. Agen No. 1", 0m, "admin");

    [Fact]
    public async Task Creates_mediator_and_returns_id()
    {
        var repo    = new FakeMediatorRepository();
        var query   = new FakeMediatorQueryRepository { NameExists = false };
        var dealer  = new FakeCurrentDealer(DealerId);
        var handler = new CreateMediatorCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("Agen Utama", repo.Saved!.Name);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task Throws_when_name_already_exists()
    {
        var repo    = new FakeMediatorRepository();
        var query   = new FakeMediatorQueryRepository { NameExists = true };
        var dealer  = new FakeCurrentDealer(DealerId);
        var handler = new CreateMediatorCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeMediatorRepository : IMediatorRepository
    {
        public Domain.Mediator.Mediator? Saved { get; private set; }

        public Task<Domain.Mediator.Mediator?> GetByIdAsync(MediatorId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Mediator.Mediator?>(null);

        public Task SaveAsync(Domain.Mediator.Mediator mediator, CancellationToken ct = default)
        {
            Saved = mediator;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeMediatorQueryRepository : IMediatorQueryRepository
    {
        public bool NameExists { get; init; }

        public Task<MediatorDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<MediatorDto?>(null);

        public Task<IReadOnlyList<MediatorDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<MediatorDto>>([]);

        public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
            => Task.FromResult(NameExists);

        public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid  DealerId      => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool  IsSuperAdmin   => false;
    }
}
