using Cassandra.Application.Commands.So.CreateSo;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.So;
using Cassandra.Application.DTOs.So;
using Cassandra.Domain.Common;
using Cassandra.Domain.So;

namespace Cassandra.Tests.So;

public class CreateSoCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateSoCommand DefaultCommand(string soNumber = "SO-001") =>
        new(
            soNumber,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            SoPaymentType.CASH,
            Guid.NewGuid(),
            0m, 0m, 0m,
            [new CreateSoItemRequest(Guid.NewGuid(), Guid.NewGuid(), 1, 25_000_000m)],
            "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenSoNumberIsUnique()
    {
        var repo = new FakeSoRepository();
        var query = new FakeSoQueryRepository { SoNumberExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateSoCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal(DealerId, repo.Saved!.DealerId);
        Assert.Equal("SO-001", repo.Saved.SoNumber);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenSoNumberExists()
    {
        var repo = new FakeSoRepository();
        var query = new FakeSoQueryRepository { SoNumberExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateSoCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    [Fact]
    public async Task HandleAsync_CalculatesFinancialFieldsCorrectly()
    {
        var repo = new FakeSoRepository();
        var query = new FakeSoQueryRepository { SoNumberExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateSoCommandHandler(repo, query, dealer);

        // Items: 2 units at 10M each = 20M total; subsidi=0; cashDiscount=5%
        var command = new CreateSoCommand(
            "SO-001",
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            SoPaymentType.CASH,
            Guid.NewGuid(),
            0m, 5m, 0m,
            [new CreateSoItemRequest(Guid.NewGuid(), Guid.NewGuid(), 2, 10_000_000m)],
            "admin");

        await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // total = 20M, subsidi=0, subtotal = 20M * 5% = 1M
        // ppn = (20M - 0 - 1M) * 0.1 = 1.9M
        // totalAmount = 20M - 0 - 1M + 1.9M = 20.9M
        Assert.NotNull(repo.Saved);
        Assert.Equal(20_000_000m, repo.Saved!.Total);
        Assert.Equal(1_900_000m, repo.Saved.PPn);
        Assert.Equal(20_900_000m, repo.Saved.TotalAmount);
    }

    // ── Fakes ──────────────────────────────────────────────────────────────────

    private sealed class FakeSoRepository : ISoRepository
    {
        public Domain.So.So? Saved { get; private set; }

        public Task<Domain.So.So?> GetByIdAsync(SoId id, CancellationToken ct = default)
            => Task.FromResult<Domain.So.So?>(null);

        public Task SaveAsync(Domain.So.So so, CancellationToken ct = default)
        {
            Saved = so;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeSoQueryRepository : ISoQueryRepository
    {
        public bool SoNumberExists { get; init; }

        public Task<IReadOnlyList<SoDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<SoDto>>([]);

        public Task<SoDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<SoDto?>(null);

        public Task<SoDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<SoDto?>(null);

        public Task<bool> SoNumberExistsAsync(string soNumber, CancellationToken ct = default)
            => Task.FromResult(SoNumberExists);

        public Task<bool> IsSoAktifAsync(Guid soId, CancellationToken ct = default)
            => Task.FromResult(true);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
