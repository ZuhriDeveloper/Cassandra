using Cassandra.Application.Commands.Stnk.ProcessStnk;
using Cassandra.Application.Contracts.Stnk;
using Cassandra.Application.DTOs.Stnk;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;

namespace Cassandra.Tests.Stnk;

public class ProcessStnkCommandHandlerTests
{
    private static readonly Guid DealerId              = Guid.NewGuid();
    private static readonly Guid RegistrasiPenjualanId = Guid.NewGuid();
    private static readonly Guid BiroId                = Guid.NewGuid();

    private static Domain.Stnk.Stnk BuildStnk() =>
        Domain.Stnk.Stnk.Create(
            RegistrasiPenjualanId,
            DateOnly.FromDateTime(DateTime.Today),
            "Budi Santoso",
            "Jl. Merdeka",
            "admin",
            DealerId);

    private static ProcessStnkCommand DefaultCommand(Guid stnkId) =>
        new(stnkId, DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");

    [Fact]
    public async Task HandleAsync_ProcessesStnk_HappyPath()
    {
        var stnk = BuildStnk();
        var repo = new FakeStnkRepository(stnk);
        var handler = new ProcessStnkCommandHandler(repo);

        await handler.HandleAsync(DefaultCommand(stnk.Id.Value), TestContext.Current.CancellationToken);

        Assert.Equal(StnkStatus.PROCESS, repo.Saved?.Status);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenStnkNotFound()
    {
        var repo = new FakeStnkRepository(null);
        var handler = new ProcessStnkCommandHandler(repo);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(Guid.NewGuid()), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenWrongStatus()
    {
        var stnk = BuildStnk();
        // Advance to PROCESS first
        stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");
        var repo = new FakeStnkRepository(stnk);
        var handler = new ProcessStnkCommandHandler(repo);

        // Cannot process again from PROCESS status
        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(stnk.Id.Value), TestContext.Current.CancellationToken));
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeStnkRepository(Domain.Stnk.Stnk? stored) : IStnkRepository
    {
        public Domain.Stnk.Stnk? Saved { get; private set; }

        public Task<Domain.Stnk.Stnk?> GetByIdAsync(StnkId id, CancellationToken ct = default)
            => Task.FromResult(stored?.Id == id ? stored : null);

        public Task SaveAsync(Domain.Stnk.Stnk stnk, CancellationToken ct = default)
        {
            Saved = stnk;
            return Task.CompletedTask;
        }
    }
}
