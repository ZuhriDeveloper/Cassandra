using Cassandra.Application.Commands.DiscountCash.CreateDiscountCash;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.DiscountCash;
using Cassandra.Application.DTOs.DiscountCash;
using Cassandra.Domain.Common;
using Cassandra.Domain.DiscountCash;

namespace Cassandra.Tests.DiscountCash;

public class CreateDiscountCashCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid TipeMotorId = Guid.NewGuid();

    private static CreateDiscountCashCommand DefaultCommand() =>
        new(TipeMotorId, 500_000m, 300_000m, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenTipeMotorIdIsUnique()
    {
        var repo = new FakeDiscountCashRepository();
        var query = new FakeDiscountCashQueryRepository { TipeMotorExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateDiscountCashCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal(TipeMotorId, repo.Saved!.TipeMotorId);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenTipeMotorIdAlreadyExists()
    {
        var repo = new FakeDiscountCashRepository();
        var query = new FakeDiscountCashQueryRepository { TipeMotorExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateDiscountCashCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeDiscountCashRepository : IDiscountCashRepository
    {
        public Domain.DiscountCash.DiscountCash? Saved { get; private set; }

        public Task<Domain.DiscountCash.DiscountCash?> GetByIdAsync(DiscountCashId id, CancellationToken ct = default)
            => Task.FromResult<Domain.DiscountCash.DiscountCash?>(null);

        public Task SaveAsync(Domain.DiscountCash.DiscountCash discountCash, CancellationToken ct = default)
        {
            Saved = discountCash;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeDiscountCashQueryRepository : IDiscountCashQueryRepository
    {
        public bool TipeMotorExists { get; init; }

        public Task<DiscountCashDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<DiscountCashDto?>(null);

        public Task<IReadOnlyList<DiscountCashDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<DiscountCashDto>>([]);

        public Task<bool> TipeMotorIdExistsAsync(Guid tipeMotorId, CancellationToken ct = default)
            => Task.FromResult(TipeMotorExists);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
