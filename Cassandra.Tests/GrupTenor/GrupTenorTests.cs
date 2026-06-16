using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTenor;
using Cassandra.Domain.GrupTenor.Events;

namespace Cassandra.Tests.GrupTenor;

public class GrupTenorTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.GrupTenor.GrupTenor MakeGt(
        string code = "GT01",
        string name = "Group Tenor 1") =>
        Domain.GrupTenor.GrupTenor.Create(code, name, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var gt = MakeGt();

        Assert.Single(gt.DomainEvents);
        var evt = Assert.IsType<GrupTenorCreated>(gt.DomainEvents[0]);

        Assert.Equal("GT01", evt.Code);
        Assert.Equal("Group Tenor 1", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("GT01", gt.Code);
        Assert.True(gt.IsActive);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var gt = MakeGt(code: "gt01");
        Assert.Equal("GT01", gt.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeGt(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeGt(name: name));
    }

    [Fact]
    public void Update_RaisesEvent_WhenNameChanges()
    {
        var gt = MakeGt();
        gt.ClearDomainEvents();

        gt.Update("Group Tenor Baru", "admin");

        Assert.IsType<GrupTenorUpdated>(Assert.Single(gt.DomainEvents));
        Assert.Equal("Group Tenor Baru", gt.Name);
    }

    [Fact]
    public void Update_IsNoop_WhenNameSame()
    {
        var gt = MakeGt();
        gt.ClearDomainEvents();

        gt.Update("Group Tenor 1", "admin");

        Assert.Empty(gt.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var gt = MakeGt();
        gt.ClearDomainEvents();

        gt.Deactivate("admin");

        Assert.False(gt.IsActive);
        Assert.IsType<GrupTenorDeactivated>(Assert.Single(gt.DomainEvents));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var gt = MakeGt();
        gt.Deactivate("admin");
        gt.ClearDomainEvents();

        gt.Activate("admin");

        Assert.True(gt.IsActive);
        Assert.IsType<GrupTenorActivated>(Assert.Single(gt.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var gt = MakeGt();
        gt.Update("Group Tenor Baru", "admin");
        gt.Deactivate("admin");

        var replayed = Domain.GrupTenor.GrupTenor.Reconstitute(gt.DomainEvents);

        Assert.Equal(gt.Id, replayed.Id);
        Assert.Equal(gt.Code, replayed.Code);
        Assert.Equal(gt.Name, replayed.Name);
        Assert.Equal(gt.IsActive, replayed.IsActive);
        Assert.Equal(gt.DealerId, replayed.DealerId);
    }
}
