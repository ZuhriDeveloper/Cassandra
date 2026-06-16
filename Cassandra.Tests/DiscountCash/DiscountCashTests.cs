using Cassandra.Domain.Common;
using Cassandra.Domain.DiscountCash;
using Cassandra.Domain.DiscountCash.Events;

namespace Cassandra.Tests.DiscountCash;

public class DiscountCashTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid TipeMotorId = Guid.NewGuid();

    private static Domain.DiscountCash.DiscountCash MakeDc(
        Guid? tipeMotorId = null,
        decimal directDiscount = 500_000m,
        decimal channelDiscount = 300_000m) =>
        Domain.DiscountCash.DiscountCash.Create(
            tipeMotorId ?? TipeMotorId, directDiscount, channelDiscount, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var dc = MakeDc();

        Assert.Single(dc.DomainEvents);
        var evt = Assert.IsType<DiscountCashCreated>(dc.DomainEvents[0]);

        Assert.Equal(TipeMotorId, evt.TipeMotorId);
        Assert.Equal(500_000m, evt.DirectDiscount);
        Assert.Equal(300_000m, evt.ChannelDiscount);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(TipeMotorId, dc.TipeMotorId);
        Assert.True(dc.IsActive);
    }

    [Fact]
    public void Create_ThrowsWhen_TipeMotorIdEmpty()
    {
        Assert.Throws<DomainException>(() => MakeDc(tipeMotorId: Guid.Empty));
    }

    [Fact]
    public void Create_ThrowsWhen_DirectDiscountNegative()
    {
        Assert.Throws<DomainException>(() => MakeDc(directDiscount: -1m));
    }

    [Fact]
    public void Create_ThrowsWhen_ChannelDiscountNegative()
    {
        Assert.Throws<DomainException>(() => MakeDc(channelDiscount: -1m));
    }

    [Fact]
    public void Create_AllowsZeroDiscount()
    {
        var dc = MakeDc(directDiscount: 0m, channelDiscount: 0m);
        Assert.Equal(0m, dc.DirectDiscount);
    }

    [Fact]
    public void Update_RaisesEvent_WhenDataChanges()
    {
        var dc = MakeDc();
        dc.ClearDomainEvents();

        dc.Update(600_000m, 400_000m, "admin");

        Assert.IsType<DiscountCashUpdated>(Assert.Single(dc.DomainEvents));
        Assert.Equal(600_000m, dc.DirectDiscount);
        Assert.Equal(400_000m, dc.ChannelDiscount);
    }

    [Fact]
    public void Update_IsNoop_WhenAllSame()
    {
        var dc = MakeDc();
        dc.ClearDomainEvents();

        dc.Update(500_000m, 300_000m, "admin");

        Assert.Empty(dc.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var dc = MakeDc();
        dc.ClearDomainEvents();

        dc.Deactivate("admin");

        Assert.False(dc.IsActive);
        Assert.IsType<DiscountCashDeactivated>(Assert.Single(dc.DomainEvents));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var dc = MakeDc();
        dc.Deactivate("admin");
        dc.ClearDomainEvents();

        dc.Activate("admin");

        Assert.True(dc.IsActive);
        Assert.IsType<DiscountCashActivated>(Assert.Single(dc.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var dc = MakeDc();
        dc.Update(600_000m, 400_000m, "admin");
        dc.Deactivate("admin");

        var replayed = Domain.DiscountCash.DiscountCash.Reconstitute(dc.DomainEvents);

        Assert.Equal(dc.Id, replayed.Id);
        Assert.Equal(dc.TipeMotorId, replayed.TipeMotorId);
        Assert.Equal(dc.DirectDiscount, replayed.DirectDiscount);
        Assert.Equal(dc.ChannelDiscount, replayed.ChannelDiscount);
        Assert.Equal(dc.IsActive, replayed.IsActive);
    }
}
