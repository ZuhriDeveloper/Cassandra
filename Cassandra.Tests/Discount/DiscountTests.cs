using Cassandra.Domain.Common;
using Cassandra.Domain.Discount;
using Cassandra.Domain.Discount.Events;

namespace Cassandra.Tests.Discount;

public class DiscountTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid DhlId = Guid.NewGuid();

    private static Domain.Discount.Discount MakeDiscount(
        Guid? dhlId = null,
        string level = "GOLD") =>
        Domain.Discount.Discount.Create(dhlId ?? DhlId, level, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var discount = MakeDiscount();

        Assert.Single(discount.DomainEvents);
        var evt = Assert.IsType<DiscountCreated>(discount.DomainEvents[0]);

        Assert.Equal(DhlId, evt.DaftarHargaLeasingId);
        Assert.Equal("GOLD", evt.Level);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("GOLD", discount.Level);
        Assert.True(discount.IsActive);
        Assert.Empty(discount.Items);
    }

    [Fact]
    public void Create_ThrowsWhen_DhlIdEmpty()
    {
        Assert.Throws<DomainException>(() => MakeDiscount(dhlId: Guid.Empty));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_LevelEmpty(string level)
    {
        Assert.Throws<DomainException>(() => MakeDiscount(level: level));
    }

    [Fact]
    public void SetItems_SetsItemsOnAggregate()
    {
        var discount = MakeDiscount();
        discount.ClearDomainEvents();

        var items = new List<DiscountLineItem>
        {
            new(Guid.NewGuid(), 500_000m),
            new(Guid.NewGuid(), 750_000m)
        };
        discount.SetItems(items, "admin");

        Assert.IsType<DiscountItemsSet>(Assert.Single(discount.DomainEvents));
        Assert.Equal(2, discount.Items.Count);
        Assert.Equal(500_000m, discount.Items[0].Amount);
    }

    [Fact]
    public void Update_RaisesEvent_WhenDataChanges()
    {
        var discount = MakeDiscount();
        discount.ClearDomainEvents();

        discount.Update(DhlId, "PLATINUM", "admin");

        Assert.IsType<DiscountUpdated>(Assert.Single(discount.DomainEvents));
        Assert.Equal("PLATINUM", discount.Level);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var discount = MakeDiscount();
        discount.ClearDomainEvents();

        discount.Deactivate("admin");

        Assert.False(discount.IsActive);
        Assert.IsType<DiscountDeactivated>(Assert.Single(discount.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresStateWithItems()
    {
        var discount = MakeDiscount();
        var items = new List<DiscountLineItem> { new(Guid.NewGuid(), 500_000m) };
        discount.SetItems(items, "admin");
        discount.Deactivate("admin");

        var replayed = Domain.Discount.Discount.Reconstitute(discount.DomainEvents);

        Assert.Equal(discount.Id, replayed.Id);
        Assert.Equal(discount.Level, replayed.Level);
        Assert.Equal(discount.IsActive, replayed.IsActive);
        Assert.Single(replayed.Items);
        Assert.Equal(500_000m, replayed.Items[0].Amount);
    }
}
