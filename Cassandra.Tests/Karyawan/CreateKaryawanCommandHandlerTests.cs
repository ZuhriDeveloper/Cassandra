using Cassandra.Application.Commands.Karyawan.CreateKaryawan;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Application.DTOs.Karyawan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;

namespace Cassandra.Tests.Karyawan;

public class CreateKaryawanCommandHandlerTests
{
    private static readonly Guid     DealerId  = Guid.NewGuid();
    private static readonly Guid     JabatanId = Guid.NewGuid();
    private static readonly DateOnly HireDate  = new(2024, 1, 15);

    private static CreateKaryawanCommand DefaultCommand(string email = "budi@cassandra.local") =>
        new("Budi Santoso", email, "3201234567890001",
            Gender.Male, HireDate, "08123456789", null, "Jl. Merdeka No. 1",
            0m, JabatanId, "admin@cassandra.local");

    [Fact]
    public async Task Creates_karyawan_and_returns_id()
    {
        var repo         = new FakeKaryawanRepository();
        var query        = new FakeKaryawanQueryRepository { EmailExists = false };
        var dealer       = new FakeCurrentDealer(DealerId);
        var handler      = new CreateKaryawanCommandHandler(repo, query, dealer);

        var id = await handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(repo.Saved);
        Assert.Equal("Budi Santoso", repo.Saved!.Name);
        Assert.Equal(DealerId, repo.Saved.DealerId);
        Assert.Equal(id, repo.Saved.Id.Value);
    }

    [Fact]
    public async Task Throws_when_email_already_exists()
    {
        var repo    = new FakeKaryawanRepository();
        var query   = new FakeKaryawanQueryRepository { EmailExists = true };
        var dealer  = new FakeCurrentDealer(DealerId);
        var handler = new CreateKaryawanCommandHandler(repo, query, dealer);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(), TestContext.Current.CancellationToken));

        Assert.Null(repo.Saved);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeKaryawanRepository : IKaryawanRepository
    {
        public Domain.Karyawan.Karyawan? Saved { get; private set; }

        public Task<Domain.Karyawan.Karyawan?> GetByIdAsync(KaryawanId id, CancellationToken ct = default)
            => Task.FromResult<Domain.Karyawan.Karyawan?>(null);

        public Task SaveAsync(Domain.Karyawan.Karyawan karyawan, CancellationToken ct = default)
        {
            Saved = karyawan;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeKaryawanQueryRepository : IKaryawanQueryRepository
    {
        public bool EmailExists { get; init; }

        public Task<KaryawanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => Task.FromResult<KaryawanDto?>(null);

        public Task<IReadOnlyList<KaryawanDto>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<KaryawanDto>>([]);

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
            => Task.FromResult(EmailExists);

        public Task<bool> EmailExistsExcludingAsync(string email, Guid excludeId, CancellationToken ct = default)
            => Task.FromResult(false);
    }

    private sealed class FakeCurrentDealer(Guid dealerId) : ICurrentDealer
    {
        public Guid DealerId     => dealerId;
        public Guid? DealerIdOrNull => dealerId;
        public bool IsSuperAdmin => false;
    }
}
