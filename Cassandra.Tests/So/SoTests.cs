using Cassandra.Domain.Common;
using Cassandra.Domain.So;
using Cassandra.Domain.So.Events;

namespace Cassandra.Tests.So;

public class SoTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid MetodeKeuanganId = Guid.NewGuid();

    private static List<SoItemValue> DefaultItems() =>
    [
        new SoItemValue(Guid.NewGuid(), Guid.NewGuid(), 2, 25_000_000m)
    ];

    private static Domain.So.So MakeSo(
        string soNumber = "SO-001",
        string paymentType = SoPaymentType.CASH,
        List<SoItemValue>? items = null)
    {
        var itemList = items ?? DefaultItems();
        var total = itemList.Sum(i => i.Qty * i.NettPrice);
        var qtyUnit = itemList.Sum(i => i.Qty);
        return Domain.So.So.Create(
            soNumber, DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            paymentType, MetodeKeuanganId,
            total, 0m, 0m, total * 0.1m, total + total * 0.1m, 0m,
            qtyUnit, itemList, "admin", DealerId);
    }

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var so = MakeSo();

        Assert.Single(so.DomainEvents);
        var evt = Assert.IsType<SoCreated>(so.DomainEvents[0]);

        Assert.Equal("SO-001", evt.SoNumber);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(SoStatus.AKTIF, so.Status);
        Assert.False(so.IsDeleted);
        Assert.Equal(2, so.QtyUnit);
    }

    [Fact]
    public void Create_NormalisesSoNumberToUppercase()
    {
        var so = MakeSo(soNumber: "so-001");
        Assert.Equal("SO-001", so.SoNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_SoNumberEmpty(string soNumber)
    {
        Assert.Throws<DomainException>(() => MakeSo(soNumber: soNumber));
    }

    [Fact]
    public void Create_ThrowsWhen_ItemsEmpty()
    {
        Assert.Throws<DomainException>(() => MakeSo(items: []));
    }

    [Theory]
    [InlineData("TUNAI")]
    [InlineData("KREDIT")]
    [InlineData("")]
    public void Create_ThrowsWhen_InvalidPaymentType(string paymentType)
    {
        Assert.Throws<DomainException>(() => MakeSo(paymentType: paymentType));
    }

    [Fact]
    public void ChangeStatus_RaisesEvent()
    {
        var so = MakeSo();
        so.ClearDomainEvents();

        so.ChangeStatus(SoStatus.SELESAI, "admin");

        var evt = Assert.IsType<SoStatusChanged>(Assert.Single(so.DomainEvents));
        Assert.Equal(SoStatus.SELESAI, evt.Status);
        Assert.Equal(SoStatus.SELESAI, so.Status);
    }

    [Fact]
    public void Delete_RaisesEvent_WhenAktif()
    {
        var so = MakeSo();
        so.ClearDomainEvents();

        so.Delete("admin");

        var evt = Assert.IsType<SoDeleted>(Assert.Single(so.DomainEvents));
        Assert.Equal("admin", evt.DeletedBy);
        Assert.True(so.IsDeleted);
    }

    [Fact]
    public void Delete_ThrowsWhen_NotAktif()
    {
        var so = MakeSo();
        so.ChangeStatus(SoStatus.SELESAI, "admin");

        Assert.Throws<DomainException>(() => so.Delete("admin"));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var so = MakeSo();
        so.ChangeStatus(SoStatus.SELESAI, "admin");

        var replayed = Domain.So.So.Reconstitute(so.DomainEvents);

        Assert.Equal(so.Id, replayed.Id);
        Assert.Equal(so.SoNumber, replayed.SoNumber);
        Assert.Equal(SoStatus.SELESAI, replayed.Status);
        Assert.Equal(so.DealerId, replayed.DealerId);
    }
}
