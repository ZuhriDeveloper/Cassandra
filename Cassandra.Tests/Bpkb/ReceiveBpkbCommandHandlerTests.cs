using Cassandra.Application.Commands.Bpkb.ReceiveBpkb;
using Cassandra.Application.Contracts.Bpkb;
using Cassandra.Application.DTOs.Bpkb;
using Cassandra.Domain.Bpkb;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.Bpkb;

public class ReceiveBpkbCommandHandlerTests
{
    private static readonly Guid DealerId              = Guid.NewGuid();
    private static readonly Guid RegistrasiPenjualanId = Guid.NewGuid();
    private static readonly Guid StnkId               = Guid.NewGuid();

    private static Domain.Bpkb.Bpkb DefaultBpkb() =>
        Domain.Bpkb.Bpkb.Create(
            RegistrasiPenjualanId,
            StnkId,
            DateOnly.FromDateTime(DateTime.Today),
            "admin",
            DealerId);

    [Fact]
    public async Task HandleAsync_ReceivesBpkb_HappyPath()
    {
        var bpkb = DefaultBpkb();
        var repo = new FakeBpkbRepository(bpkb);
        var handler = new ReceiveBpkbCommandHandler(repo);

        await handler.HandleAsync(
            new ReceiveBpkbCommand(bpkb.Id.Value, DateOnly.FromDateTime(DateTime.Today), "BPKB-001", "BK-001", "admin"),
            TestContext.Current.CancellationToken);

        Assert.Equal(BpkbStatus.RECEIVE, repo.Saved?.Status);
        Assert.Equal("BPKB-001", repo.Saved?.BpkbNumber);
        Assert.Equal("BK-001", repo.Saved?.BookNumber);
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenBpkbNotFound()
    {
        var repo = new FakeBpkbRepository(null);
        var handler = new ReceiveBpkbCommandHandler(repo);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(
                new ReceiveBpkbCommand(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today), "BPKB-001", "BK-001", "admin"),
                TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task HandleAsync_Throws_WhenWrongStatus()
    {
        var bpkb = DefaultBpkb();
        // Advance to RECEIVE first
        bpkb.Receive("BPKB-001", "BK-001", DateOnly.FromDateTime(DateTime.Today), "admin");

        var repo = new FakeBpkbRepository(bpkb);
        var handler = new ReceiveBpkbCommandHandler(repo);

        // Cannot receive again
        await Assert.ThrowsAsync<DomainException>(() =>
            handler.HandleAsync(
                new ReceiveBpkbCommand(bpkb.Id.Value, DateOnly.FromDateTime(DateTime.Today), "BPKB-002", "BK-002", "admin"),
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
