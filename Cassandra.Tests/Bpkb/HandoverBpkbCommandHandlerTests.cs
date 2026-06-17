using Cassandra.Application.Commands.Bpkb.HandoverBpkb;
using Cassandra.Application.Contracts.Bpkb;
using Cassandra.Application.DTOs.Bpkb;
using Cassandra.Domain.Bpkb;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.Bpkb;

public class HandoverBpkbCommandHandlerTests
{
    private static readonly Guid DealerId              = Guid.NewGuid();
    private static readonly Guid RegistrasiPenjualanId = Guid.NewGuid();
    private static readonly Guid StnkId               = Guid.NewGuid();

    private static Domain.Bpkb.Bpkb BuildReceivedBpkb()
    {
        var bpkb = Domain.Bpkb.Bpkb.Create(
            RegistrasiPenjualanId,
            StnkId,
            DateOnly.FromDateTime(DateTime.Today),
            "admin",
            DealerId);
        bpkb.Receive("BPKB-001", "BK-001", DateOnly.FromDateTime(DateTime.Today), "admin");
        return bpkb;
    }

    [Fact]
    public async Task HandleAsync_HandsOverBpkb_HappyPath()
    {
        var bpkb = BuildReceivedBpkb();
        var repo = new FakeBpkbRepository(bpkb);
        var handler = new HandoverBpkbCommandHandler(repo);

        await handler.HandleAsync(
            new HandoverBpkbCommand(bpkb.Id.Value, DateOnly.FromDateTime(DateTime.Today), "Budi Santoso", "admin"),
            TestContext.Current.CancellationToken);

        Assert.Equal(BpkbStatus.HANDOVER, repo.Saved?.Status);
        Assert.Equal("Budi Santoso", repo.Saved?.Receiver);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenBpkbNotFound()
    {
        var repo = new FakeBpkbRepository(null);
        var handler = new HandoverBpkbCommandHandler(repo);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(
                new HandoverBpkbCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), "Budi", "admin"),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenWrongStatus()
    {
        // Still in REQUEST — cannot hand over
        var bpkb = Domain.Bpkb.Bpkb.Create(
            RegistrasiPenjualanId,
            StnkId,
            DateOnly.FromDateTime(DateTime.Today),
            "admin",
            DealerId);

        var repo = new FakeBpkbRepository(bpkb);
        var handler = new HandoverBpkbCommandHandler(repo);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(
                new HandoverBpkbCommand(bpkb.Id.Value, DateOnly.FromDateTime(DateTime.Today), "Budi", "admin"),
                TestContext.Current.CancellationToken));
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private sealed class FakeBpkbRepository(Domain.Bpkb.Bpkb? stored) : IBpkbRepository
    {
        public Domain.Bpkb.Bpkb? Saved { get; private set; }

        public Task<Domain.Bpkb.Bpkb?> GetByIdAsync(BpkbId id, CancellationToken ct = default)
            => Task.FromResult(stored?.Id == id ? stored : null);

        public Task SaveAsync(Domain.Bpkb.Bpkb bpkb, CancellationToken ct = default)
        {
            Saved = bpkb;
            return Task.CompletedTask;
        }
    }
}
