using Cassandra.Domain.Common;
using Cassandra.Domain.MetodeKeuangan;
using Cassandra.Domain.MetodeKeuangan.Events;

namespace Cassandra.Tests.MetodeKeuangan;

public class MetodeKeuanganTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.MetodeKeuangan.MetodeKeuangan MakeMk(
        string code = "KAS",
        string name = "Kas") =>
        Domain.MetodeKeuangan.MetodeKeuangan.Create(code, name, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var mk = MakeMk();

        Assert.Single(mk.DomainEvents);
        var evt = Assert.IsType<MetodeKeuanganCreated>(mk.DomainEvents[0]);

        Assert.Equal("KAS", evt.Code);
        Assert.Equal("Kas", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("KAS", mk.Code);
        Assert.Equal("Kas", mk.Name);
        Assert.True(mk.IsActive);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var mk = MakeMk(code: "kas");
        Assert.Equal("KAS", mk.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeMk(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeMk(name: name));
    }

    [Fact]
    public void Update_RaisesEvent_WhenNameChanges()
    {
        var mk = MakeMk();
        mk.ClearDomainEvents();

        mk.Update("Kredit", "admin");

        var evt = Assert.IsType<MetodeKeuanganUpdated>(Assert.Single(mk.DomainEvents));
        Assert.Equal("Kredit", evt.Name);
        Assert.Equal("Kredit", mk.Name);
    }

    [Fact]
    public void Update_IsNoop_WhenNameSame()
    {
        var mk = MakeMk();
        mk.ClearDomainEvents();

        mk.Update("Kas", "admin");

        Assert.Empty(mk.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var mk = MakeMk();
        mk.ClearDomainEvents();

        mk.Deactivate("admin");

        Assert.False(mk.IsActive);
        Assert.IsType<MetodeKeuanganDeactivated>(Assert.Single(mk.DomainEvents));
    }

    [Fact]
    public void Deactivate_ThrowsWhen_AlreadyInactive()
    {
        var mk = MakeMk();
        mk.Deactivate("admin");

        Assert.Throws<DomainException>(() => mk.Deactivate("admin"));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var mk = MakeMk();
        mk.Deactivate("admin");
        mk.ClearDomainEvents();

        mk.Activate("admin");

        Assert.True(mk.IsActive);
        Assert.IsType<MetodeKeuanganActivated>(Assert.Single(mk.DomainEvents));
    }

    [Fact]
    public void Activate_ThrowsWhen_AlreadyActive()
    {
        var mk = MakeMk();
        Assert.Throws<DomainException>(() => mk.Activate("admin"));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var mk = MakeMk();
        mk.Update("Kredit", "admin");
        mk.Deactivate("admin");

        var replayed = Domain.MetodeKeuangan.MetodeKeuangan.Reconstitute(mk.DomainEvents);

        Assert.Equal(mk.Id, replayed.Id);
        Assert.Equal(mk.Code, replayed.Code);
        Assert.Equal(mk.Name, replayed.Name);
        Assert.Equal(mk.IsActive, replayed.IsActive);
        Assert.Equal(mk.DealerId, replayed.DealerId);
    }
}
