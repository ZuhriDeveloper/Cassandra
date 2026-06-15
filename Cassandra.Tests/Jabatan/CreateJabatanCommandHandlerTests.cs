using Cassandra.Application.Commands.Jabatan.CreateJabatan;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Jabatan;
using Cassandra.Application.DTOs.Jabatan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Jabatan;

namespace Cassandra.Tests.Jabatan;

public class CreateJabatanCommandHandlerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    [Fact]
    public async Task Creates_jabatan_and_returns_id()
    {
        var repo = new FakeJabatanRepository();
        var query = new FakeJabatanQueryRepository { NameExists = false };
        var currentDealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateJabatanCommandHandler(repo, query, currentDealer);

        var id = await handler.HandleAsync(
            new CreateJabatanCommand("Kepala Mekanik", "Deskripsi", "admin@cassandra.local"),
            TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("Kepala Mekanik", repo.Saved!.Name);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task Throws_when_name_already_exists()
    {
        var repo = new FakeJabatanRepository();
        var query = new FakeJabatanQueryRepository { NameExists = true };
        var currentDealer = new FakeCurrentDealer(DealerId);
        var handler = new CreateJabatanCommandHandler(repo, query, currentDealer);

        await Assert.ThrowsAsync<DomainException>(() => handler.HandleAsync(
            new CreateJabatanCommand("Kepala Mekanik", "Deskripsi", "admin@cassandra.local"),
            TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeJabatanRepository : IJabatanRepository
    {
        public Domain.Jabatan.Jabatan? Saved { get; private set; }

        public Task<Domain.Jabatan.Jabatan?> GetByIdAsync(JabatanId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Jabatan.Jabatan?>(null);

        public Task SaveAsync(Domain.Jabatan.Jabatan jabatan, CancellationToken ct = default)
        {
            Saved = jabatan;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeJabatanQueryRepository : IJabatanQueryRepository
    {
        public bool NameExists { get; init; }

        public Task<JabatanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<JabatanDto?>(null);

        public Task<IReadOnlyList<JabatanDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<JabatanDto>>([]);

        public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
            => Task.FromResult(NameExists);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
