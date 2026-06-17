using Cassandra.Application.Commands.Stnk.HandoverStnk;
using Cassandra.Application.Contracts.Stnk;
using Cassandra.Application.DTOs.Stnk;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;

namespace Cassandra.Tests.Stnk;

public class HandoverStnkCommandHandlerTests
{
    private static readonly Guid DealerId              = Guid.NewGuid();
    private static readonly Guid RegistrasiPenjualanId = Guid.NewGuid();
    private static readonly Guid BiroId                = Guid.NewGuid();

    private static Domain.Stnk.Stnk BuildReceivedStnk()
    {
        var stnk = Domain.Stnk.Stnk.Create(
            RegistrasiPenjualanId,
            DateOnly.FromDateTime(DateTime.Today),
            "Budi Santoso",
            "Jl. Merdeka",
            "admin",
            DealerId);
        stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");
        stnk.Receive(
            DateOnly.FromDateTime(DateTime.Today), "B1234XY", BiroId, "STNK-001",
            500_000m, 50_000m, 0m, "INV-001", null, 100_000m, 50_000m, 25_000m, 10_000m, 20_000m, 5_000m,
            true, "admin");
        return stnk;
    }

    [Fact]
    public async Task HandleAsync_HandsOverStnk_HappyPath()
    {
        var stnk = BuildReceivedStnk();
        var repo = new FakeStnkRepository(stnk);
        var handler = new HandoverStnkCommandHandler(repo);

        await handler.HandleAsync(
            new HandoverStnkCommand(stnk.Id.Value, DateOnly.FromDateTime(DateTime.Today), "Budi Santoso", "admin"),
            TestContext.Current.CancellationToken);

        Assert.Equal(StnkStatus.HANDOVER_STNK, repo.Saved?.Status);
        Assert.Equal("Budi Santoso", repo.Saved?.StnkReceiver);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenStnkNotFound()
    {
        var repo = new FakeStnkRepository(null);
        var handler = new HandoverStnkCommandHandler(repo);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(
                new HandoverStnkCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), "Budi", "admin"),
                TestContext.Current.CancellationToken));
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
