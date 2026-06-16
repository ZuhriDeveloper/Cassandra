using Cassandra.Application.Commands.ExpenseType.CreateExpenseType;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.ExpenseType;
using Cassandra.Application.DTOs.ExpenseType;
using Cassandra.Domain.Common;
using Cassandra.Domain.ExpenseType;

namespace Cassandra.Tests.ExpenseType;

public class CreateExpenseTypeCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static CreateExpenseTypeCommand DefaultCommand(string code = "BP") =>
        new(code, "Biaya Perawatan", "admin");

    [Fact]
    public async Task HandleAsync_Succeeds_WhenCodeIsUnique()
    {
        var repo = new FakeExpenseTypeRepository();
        var query = new FakeExpenseTypeQueryRepository { CodeExists = false, NameExists = false };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateExpenseTypeCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("BP", repo.Saved!.Code);
        Assert.Equal(DealerId, repo.Saved.DealerId);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenCodeExists()
    {
        var repo = new FakeExpenseTypeRepository();
        var query = new FakeExpenseTypeQueryRepository { CodeExists = true };
        var dealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateExpenseTypeCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeExpenseTypeRepository : IExpenseTypeRepository
    {
        public Domain.ExpenseType.ExpenseType? Saved { get; private set; }

        public Task<Domain.ExpenseType.ExpenseType?> GetByIdAsync(ExpenseTypeId id, CancellationToken ct = default)
            => Task.FromResult<Domain.ExpenseType.ExpenseType?>(null);

        public Task SaveAsync(Domain.ExpenseType.ExpenseType et, CancellationToken ct = default)
        {
            Saved = et;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeExpenseTypeQueryRepository : IExpenseTypeQueryRepository
    {
        public bool CodeExists { get; init; }
        public bool NameExists { get; init; }

        public Task<ExpenseTypeDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<ExpenseTypeDto?>(null);

        public Task<IReadOnlyList<ExpenseTypeDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<ExpenseTypeDto>>([]);

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => Task.FromResult(CodeExists);

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
