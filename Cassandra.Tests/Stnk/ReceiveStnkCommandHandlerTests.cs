using Cassandra.Application.Commands.Stnk.ReceiveStnk;
using Cassandra.Application.Contracts.Stnk;
using Cassandra.Application.DTOs.Stnk;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;

namespace Cassandra.Tests.Stnk;

public class ReceiveStnkCommandHandlerTests
{
    private static readonly Guid DealerId              = Guid.NewGuid();
    private static readonly Guid RegistrasiPenjualanId = Guid.NewGuid();
    private static readonly Guid BiroId                = Guid.NewGuid();

    private static Domain.Stnk.Stnk BuildProcessedStnk()
    {
        var stnk = Domain.Stnk.Stnk.Create(
            RegistrasiPenjualanId,
            DateOnly.FromDateTime(DateTime.Today),
            "Budi Santoso",
            "Jl. Merdeka",
            "admin",
            DealerId);
        stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");
        return stnk;
    }

    private static ReceiveStnkCommand DefaultCommand(Guid stnkId) =>
        new(stnkId,
            DateOnly.FromDateTime(DateTime.Today),
            "B1234XY", BiroId, "STNK-001",
            500_000m, 50_000m, 0m, "INV-001", "DKI",
            100_000m, 50_000m, 25_000m, 10_000m, 20_000m, 5_000m,
            true, "admin");

    [Fact]
    public async Task HandleAsync_ReceivesStnk_HappyPath()
    {
        var stnk = BuildProcessedStnk();
        var repo = new FakeStnkRepository(stnk);
        var handler = new ReceiveStnkCommandHandler(repo);

        await handler.HandleAsync(DefaultCommand(stnk.Id.Value), TestContext.Current.CancellationToken);

        Assert.Equal(StnkStatus.RECEIVE, repo.Saved?.Status);
        Assert.Equal("B1234XY", repo.Saved?.PlateNumber);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenStnkNotFound()
    {
        var repo = new FakeStnkRepository(null);
        var handler = new ReceiveStnkCommandHandler(repo);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(DefaultCommand(Guid.NewGuid()), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenWrongStatus()
    {
        // Status is RECEIVE_FAKTUR (not PROCESS) — receive should fail
        var stnk = Domain.Stnk.Stnk.Create(
            RegistrasiPenjualanId,
            DateOnly.FromDateTime(DateTime.Today),
            "Budi",
            "Jl. Test",
            "admin",
            DealerId);

        var repo = new FakeStnkRepository(stnk);
        var handler = new ReceiveStnkCommandHandler(repo);

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
