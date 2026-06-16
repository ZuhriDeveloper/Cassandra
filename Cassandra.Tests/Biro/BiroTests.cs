using Cassandra.Domain.Biro;
using Cassandra.Domain.Biro.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.Biro;

public class BiroTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.Biro.Biro MakeBiro(
        string code = "BR001",
        string name = "Biro Jasa Maju",
        decimal pphRate = 2.5m)
        => Domain.Biro.Biro.Create(code, name, null, null, null, null, pphRate, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var biro = MakeBiro();

        Assert.Single(biro.DomainEvents);
        var evt = Assert.IsType<BiroCreated>(biro.DomainEvents[0]);

        Assert.Equal("BR001", evt.Code);
        Assert.Equal("Biro Jasa Maju", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("BR001", biro.Code);
        Assert.Equal("Biro Jasa Maju", biro.Name);
        Assert.Equal(2.5m, biro.PphRate);
        Assert.True(biro.IsActive);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var biro = MakeBiro(code: "br001");
        Assert.Equal("BR001", biro.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeBiro(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeBiro(name: name));
    }

    [Fact]
    public void Create_ThrowsWhen_PphRateNegative()
    {
        Assert.Throws<DomainException>(() => MakeBiro(pphRate: -1m));
    }

    [Fact]
    public void Update_RaisesEvent_WhenDataChanges()
    {
        var biro = MakeBiro();
        biro.ClearDomainEvents();

        biro.Update("Biro Jasa Sejahtera", "021-1234", null, null, null, 3.0m, "admin");

        var evt = Assert.IsType<BiroUpdated>(Assert.Single(biro.DomainEvents));
        Assert.Equal("Biro Jasa Sejahtera", evt.Name);
        Assert.Equal(3.0m, evt.PphRate);
        Assert.Equal("Biro Jasa Sejahtera", biro.Name);
    }

    [Fact]
    public void Update_IsNoop_WhenDataSame()
    {
        var biro = MakeBiro();
        biro.ClearDomainEvents();

        biro.Update("Biro Jasa Maju", null, null, null, null, 2.5m, "admin");

        Assert.Empty(biro.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var biro = MakeBiro();
        biro.ClearDomainEvents();

        biro.Deactivate("admin");

        Assert.False(biro.IsActive);
        Assert.IsType<BiroDeactivated>(Assert.Single(biro.DomainEvents));
    }

    [Fact]
    public void Deactivate_ThrowsWhen_AlreadyInactive()
    {
        var biro = MakeBiro();
        biro.Deactivate("admin");

        Assert.Throws<DomainException>(() => biro.Deactivate("admin"));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var biro = MakeBiro();
        biro.Deactivate("admin");
        biro.ClearDomainEvents();

        biro.Activate("admin");

        Assert.True(biro.IsActive);
        Assert.IsType<BiroActivated>(Assert.Single(biro.DomainEvents));
    }

    [Fact]
    public void Activate_ThrowsWhen_AlreadyActive()
    {
        var biro = MakeBiro();
        Assert.Throws<DomainException>(() => biro.Activate("admin"));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var biro = MakeBiro();
        biro.Update("Biro Baru", "021-9999", null, "Pak Budi", "Jl. Merdeka 1", 5.0m, "admin");
        biro.Deactivate("admin");

        var replayed = Domain.Biro.Biro.Reconstitute(biro.DomainEvents);

        Assert.Equal(biro.Id, replayed.Id);
        Assert.Equal(biro.Code, replayed.Code);
        Assert.Equal(biro.Name, replayed.Name);
        Assert.Equal(biro.PphRate, replayed.PphRate);
        Assert.Equal(biro.IsActive, replayed.IsActive);
        Assert.Equal(biro.DealerId, replayed.DealerId);
    }
}
