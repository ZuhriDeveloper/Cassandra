using Cassandra.Application.Commands.Discount.CreateDiscount;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Discount;
using Cassandra.Application.DTOs.Discount;
using Cassandra.Domain.Common;
using Cassandra.Domain.Discount;

namespace Cassandra.Tests.Discount;

public class CreateDiscountCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid DhlId = Guid.NewGuid();

    private static CreateDiscountCommand DefaultCommand() =>
        new(DhlId, "GOLD", "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenLevelIsUniqueForDhl()
    {
        var repo = new FakeDiscountRepository();
        var query = new FakeDiscountQueryRepository { Exists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateDiscountCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("GOLD", repo.Saved!.Level);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenLevelExistsForDhl()
    {
        var repo = new FakeDiscountRepository();
        var query = new FakeDiscountQueryRepository { Exists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateDiscountCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeDiscountRepository : IDiscountRepository
    {
        public Domain.Discount.Discount? Saved { get; private set; }

        public Task<Domain.Discount.Discount?> GetByIdAsync(DiscountId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Discount.Discount?>(null);

        public Task SaveAsync(Domain.Discount.Discount discount, CancellationToken ct = default)
        {
            Saved = discount;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeDiscountQueryRepository : IDiscountQueryRepository
    {
        public bool Exists { get; init; }

        public Task<DiscountDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<DiscountDto?>(null);

        public Task<IReadOnlyList<DiscountDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<DiscountDto>>([]);

        public Task<bool> ExistsAsync(Guid daftarHargaLeasingId, string level, CancellationToken ct = default)
            => Task.FromResult(Exists);

        public Task<bool> ExistsExcludingAsync(Guid daftarHargaLeasingId, string level, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
