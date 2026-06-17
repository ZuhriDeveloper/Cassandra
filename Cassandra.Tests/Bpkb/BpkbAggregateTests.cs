using Cassandra.Domain.Bpkb;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.Bpkb;

public class BpkbAggregateTests
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
    public void Create_SetsStatusRequest()
    {
        var bpkb = DefaultBpkb();

        Assert.Equal(BpkbStatus.REQUEST, bpkb.Status);
        Assert.Equal(RegistrasiPenjualanId, bpkb.RegistrasiPenjualanId);
        Assert.Equal(StnkId, bpkb.StnkId);
        Assert.Equal(DealerId, bpkb.DealerId);
        Assert.Equal(1, bpkb.Version);
    }

    [Fact]
    public void Receive_SetsStatusReceive()
    {
        var bpkb = DefaultBpkb();
        bpkb.Receive("BPKB-001", "BK-001", DateOnly.FromDateTime(DateTime.Today), "admin");

        Assert.Equal(BpkbStatus.RECEIVE, bpkb.Status);
        Assert.Equal("BPKB-001", bpkb.BpkbNumber);
        Assert.Equal("BK-001", bpkb.BookNumber);
    }

    [Fact]
    public void Receive_Throws_WhenNotRequest()
    {
        var bpkb = DefaultBpkb();
        bpkb.Receive("BPKB-001", "BK-001", DateOnly.FromDateTime(DateTime.Today), "admin");

        // now in RECEIVE — cannot receive again
        Assert.Throws<DomainException>(() =>
            bpkb.Receive("BPKB-002", "BK-002", DateOnly.FromDateTime(DateTime.Today), "admin"));
    }

    [Fact]
    public void HandOver_SetsStatusHandover()
    {
        var bpkb = DefaultBpkb();
        bpkb.Receive("BPKB-001", "BK-001", DateOnly.FromDateTime(DateTime.Today), "admin");
        bpkb.HandOver(DateOnly.FromDateTime(DateTime.Today), "Budi Santoso", "admin");

        Assert.Equal(BpkbStatus.HANDOVER, bpkb.Status);
        Assert.Equal("Budi Santoso", bpkb.Receiver);
    }

    [Fact]
    public void HandOver_Throws_WhenNotReceive()
    {
        var bpkb = DefaultBpkb();

        // still in REQUEST — cannot hand over
        Assert.Throws<DomainException>(() =>
            bpkb.HandOver(DateOnly.FromDateTime(DateTime.Today), "Budi", "admin"));
    }

    [Fact]
    public void Reconstitute_RestoresFullState()
    {
        var bpkb = DefaultBpkb();
        bpkb.Receive("BPKB-001", "BK-001", DateOnly.FromDateTime(DateTime.Today), "admin");
        bpkb.HandOver(DateOnly.FromDateTime(DateTime.Today), "Budi Santoso", "admin");

        var reconstituted = Domain.Bpkb.Bpkb.Reconstitute(bpkb.DomainEvents);

        Assert.Equal(BpkbStatus.HANDOVER, reconstituted.Status);
        Assert.Equal(bpkb.RegistrasiPenjualanId, reconstituted.RegistrasiPenjualanId);
        Assert.Equal(bpkb.StnkId, reconstituted.StnkId);
        Assert.Equal("Budi Santoso", reconstituted.Receiver);
        Assert.Equal(3, reconstituted.Version);
    }
}
