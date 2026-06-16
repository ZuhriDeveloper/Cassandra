using Cassandra.Domain.Common;
using Cassandra.Domain.Tenor;
using Cassandra.Domain.Tenor.Events;

namespace Cassandra.Tests.Tenor;

public class TenorTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid GrupTenorId = Guid.NewGuid();

    private static Domain.Tenor.Tenor MakeTenor(
        string code = "T12",
        string name = "12 Bulan",
        int months = 12,
        Guid? grupTenorId = null) =>
        Domain.Tenor.Tenor.Create(code, name, months, grupTenorId ?? GrupTenorId, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var tenor = MakeTenor();

        Assert.Single(tenor.DomainEvents);
        var evt = Assert.IsType<TenorCreated>(tenor.DomainEvents[0]);

        Assert.Equal("T12", evt.Code);
        Assert.Equal("12 Bulan", evt.Name);
        Assert.Equal(12, evt.Months);
        Assert.Equal(GrupTenorId, evt.GrupTenorId);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.True(tenor.IsActive);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var tenor = MakeTenor(code: "t12");
        Assert.Equal("T12", tenor.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeTenor(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeTenor(name: name));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ThrowsWhen_MonthsNotPositive(int months)
    {
        Assert.Throws<DomainException>(() => MakeTenor(months: months));
    }

    [Fact]
    public void Create_ThrowsWhen_GrupTenorIdEmpty()
    {
        Assert.Throws<DomainException>(() => MakeTenor(grupTenorId: Guid.Empty));
    }

    [Fact]
    public void Update_RaisesEvent_WhenDataChanges()
    {
        var tenor = MakeTenor();
        tenor.ClearDomainEvents();

        tenor.Update("24 Bulan", 24, GrupTenorId, "admin");

        Assert.IsType<TenorUpdated>(Assert.Single(tenor.DomainEvents));
        Assert.Equal("24 Bulan", tenor.Name);
        Assert.Equal(24, tenor.Months);
    }

    [Fact]
    public void Update_IsNoop_WhenAllSame()
    {
        var tenor = MakeTenor();
        tenor.ClearDomainEvents();

        tenor.Update("12 Bulan", 12, GrupTenorId, "admin");

        Assert.Empty(tenor.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var tenor = MakeTenor();
        tenor.ClearDomainEvents();

        tenor.Deactivate("admin");

        Assert.False(tenor.IsActive);
        Assert.IsType<TenorDeactivated>(Assert.Single(tenor.DomainEvents));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var tenor = MakeTenor();
        tenor.Deactivate("admin");
        tenor.ClearDomainEvents();

        tenor.Activate("admin");

        Assert.True(tenor.IsActive);
        Assert.IsType<TenorActivated>(Assert.Single(tenor.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var tenor = MakeTenor();
        tenor.Update("24 Bulan", 24, GrupTenorId, "admin");
        tenor.Deactivate("admin");

        var replayed = Domain.Tenor.Tenor.Reconstitute(tenor.DomainEvents);

        Assert.Equal(tenor.Id, replayed.Id);
        Assert.Equal(tenor.Code, replayed.Code);
        Assert.Equal(tenor.Name, replayed.Name);
        Assert.Equal(tenor.Months, replayed.Months);
        Assert.Equal(tenor.GrupTenorId, replayed.GrupTenorId);
        Assert.Equal(tenor.IsActive, replayed.IsActive);
    }
}
