using Cassandra.Application.Commands.Dealers.RegisterDealer;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.DTOs.Dealers;
using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers;

namespace Cassandra.Tests.Dealers;

public class RegisterDealerCommandHandlerTests
{
    [Fact]
    public async Task Registers_dealer_and_returns_id()
    {
        var repo = new FakeDealerRepository();
        var query = new FakeDealerQueryRepository { CodeExists = false };
        var handler = new RegisterDealerCommandHandler(repo, query);

        var id = await handler.HandleAsync(
            new RegisterDealerCommand("Dealer Pusat", "D1", "superadmin@cassandra.local"),
            TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("Dealer Pusat", repo.Saved!.Name);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task Throws_when_code_already_exists()
    {
        var repo = new FakeDealerRepository();
        var query = new FakeDealerQueryRepository { CodeExists = true };
        var handler = new RegisterDealerCommandHandler(repo, query);

        await Assert.ThrowsAsync<DomainException>(() => handler.HandleAsync(
            new RegisterDealerCommand("Dealer Pusat", "D1", "superadmin@cassandra.local"),
            TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    private sealed class FakeDealerRepository : IDealerRepository
    {
        public Dealer? Saved { get; private set; }

        public Task<Dealer?> GetByIdAsync(DealerId id, CancellationToken ct = default)
            => Task.FromResult<Dealer?>(null);

        public Task SaveAsync(Dealer dealer, CancellationToken ct = default)
        {
            Saved = dealer;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeDealerQueryRepository : IDealerQueryRepository
    {
        public bool CodeExists { get; init; }

        public Task<DealerDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<DealerDto?>(null);

        public Task<IReadOnlyList<DealerDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<DealerDto>>([]);

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => Task.FromResult(CodeExists);
    }
}
