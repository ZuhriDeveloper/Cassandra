using Cassandra.Domain.Common;
using Cassandra.Domain.Stock;
using Cassandra.Domain.Stock.Events;

namespace Cassandra.Tests.Stock;

public class StockTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid TipeMotorId = Guid.NewGuid();
    private static readonly Guid WarnaId = Guid.NewGuid();
    private static readonly Guid KiosId = Guid.NewGuid();
    private static readonly Guid SoId = Guid.NewGuid();
    private static readonly DateOnly SuratJalanDate = DateOnly.FromDateTime(DateTime.Today);

    private static Domain.Stock.Stock MakeStock(
        string noMesin = "M001",
        string noRangka = "R001")
    {
        return Domain.Stock.Stock.Create(
            noMesin, noRangka, TipeMotorId, WarnaId,
            KiosId, "SJ-001", SuratJalanDate, SoId, "2025", "admin", DealerId);
    }

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var stock = MakeStock();

        Assert.Single(stock.DomainEvents);
        var evt = Assert.IsType<StockCreated>(stock.DomainEvents[0]);

        Assert.Equal("M001", evt.NoMesin);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(StockStatus.TERSEDIA, stock.Status);
        Assert.True(stock.IsActive);
        Assert.Equal(KiosId, stock.KiosId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NoMesinEmpty(string noMesin)
    {
        Assert.Throws<DomainException>(() => MakeStock(noMesin: noMesin));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NoRangkaEmpty(string noRangka)
    {
        Assert.Throws<DomainException>(() => MakeStock(noRangka: noRangka));
    }

    [Fact]
    public void ChangeStatus_RaisesEvent()
    {
        var stock = MakeStock();
        stock.ClearDomainEvents();

        stock.ChangeStatus(StockStatus.DIPESAN, "admin");

        var evt = Assert.IsType<StockStatusChanged>(Assert.Single(stock.DomainEvents));
        Assert.Equal(StockStatus.DIPESAN, evt.Status);
        Assert.Equal(StockStatus.DIPESAN, stock.Status);
    }

    [Fact]
    public void MoveToKios_RaisesEvent_WhenDifferentKios()
    {
        var stock = MakeStock();
        var newKiosId = Guid.NewGuid();
        stock.ClearDomainEvents();

        stock.MoveToKios(newKiosId, "admin");

        var evt = Assert.IsType<StockMoved>(Assert.Single(stock.DomainEvents));
        Assert.Equal(newKiosId, evt.NewKiosId);
        Assert.Equal(newKiosId, stock.KiosId);
    }

    [Fact]
    public void MoveToKios_IsNoop_WhenSameKios()
    {
        var stock = MakeStock();
        stock.ClearDomainEvents();

        stock.MoveToKios(KiosId, "admin");

        Assert.Empty(stock.DomainEvents);
        Assert.Equal(KiosId, stock.KiosId);
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var stock = MakeStock();
        var newKiosId = Guid.NewGuid();
        stock.MoveToKios(newKiosId, "admin");

        var replayed = Domain.Stock.Stock.Reconstitute(stock.DomainEvents);

        Assert.Equal(stock.Id, replayed.Id);
        Assert.Equal(stock.NoMesin, replayed.NoMesin);
        Assert.Equal(newKiosId, replayed.KiosId);
        Assert.Equal(StockStatus.TERSEDIA, replayed.Status);
    }
}
