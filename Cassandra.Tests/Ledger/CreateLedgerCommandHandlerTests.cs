using Cassandra.Application.Commands.Ledger.CreateLedger;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Ledger;
using Cassandra.Application.DTOs.Ledger;
using Cassandra.Domain.Common;
using Cassandra.Domain.Ledger;

namespace Cassandra.Tests.Ledger;

public class CreateLedgerCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateLedgerCommand DefaultCommand(string name = "Kas Besar")
        => new(name, "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenNameIsUnique()
    {
        var repo = new FakeLedgerRepository();
        var query = new FakeLedgerQueryRepository { NameExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateLedgerCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("Kas Besar", repo.Saved!.Name);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenNameExists()
    {
        var repo = new FakeLedgerRepository();
        var query = new FakeLedgerQueryRepository { NameExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateLedgerCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeLedgerRepository : ILedgerRepository
    {
        public Domain.Ledger.Ledger? Saved { get; private set; }

        public Task<Domain.Ledger.Ledger?> GetByIdAsync(LedgerId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Ledger.Ledger?>(null);

        public Task SaveAsync(Domain.Ledger.Ledger ledger, CancellationToken ct = default)
        {
            Saved = ledger;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeLedgerQueryRepository : ILedgerQueryRepository
    {
        public bool NameExists { get; init; }

        public Task<LedgerDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<LedgerDto?>(null);

        public Task<IReadOnlyList<LedgerDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<LedgerDto>>([]);

        public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
            => Task.FromResult(NameExists);

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
