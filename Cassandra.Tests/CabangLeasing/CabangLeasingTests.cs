using Cassandra.Domain.CabangLeasing;
using Cassandra.Domain.CabangLeasing.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.CabangLeasing;

public class CabangLeasingTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid GlobalLeasingId = Guid.NewGuid();

    private static Domain.CabangLeasing.CabangLeasing MakeCl(
        string code = "BCA-JKT",
        string name = "BCA Jakarta",
        Guid? globalLeasingId = null) =>
        Domain.CabangLeasing.CabangLeasing.Create(
            code, name, "021-5555", null, "John", globalLeasingId ?? GlobalLeasingId, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var cl = MakeCl();

        Assert.Single(cl.DomainEvents);
        var evt = Assert.IsType<CabangLeasingCreated>(cl.DomainEvents[0]);

        Assert.Equal("BCA-JKT", evt.Code);
        Assert.Equal("BCA Jakarta", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(GlobalLeasingId, evt.GlobalLeasingId);
        Assert.Equal("BCA-JKT", cl.Code);
        Assert.True(cl.IsActive);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var cl = MakeCl(code: "bca-jkt");
        Assert.Equal("BCA-JKT", cl.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeCl(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeCl(name: name));
    }

    [Fact]
    public void Create_ThrowsWhen_GlobalLeasingIdEmpty()
    {
        Assert.Throws<DomainException>(() => MakeCl(globalLeasingId: Guid.Empty));
    }

    [Fact]
    public void Update_RaisesEvent_WhenDataChanges()
    {
        var cl = MakeCl();
        cl.ClearDomainEvents();

        cl.Update("BCA Jakarta Baru", "021-6666", null, "Jane", GlobalLeasingId, "admin");

        Assert.IsType<CabangLeasingUpdated>(Assert.Single(cl.DomainEvents));
        Assert.Equal("BCA Jakarta Baru", cl.Name);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var cl = MakeCl();
        cl.ClearDomainEvents();

        cl.Deactivate("admin");

        Assert.False(cl.IsActive);
        Assert.IsType<CabangLeasingDeactivated>(Assert.Single(cl.DomainEvents));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var cl = MakeCl();
        cl.Deactivate("admin");
        cl.ClearDomainEvents();

        cl.Activate("admin");

        Assert.True(cl.IsActive);
        Assert.IsType<CabangLeasingActivated>(Assert.Single(cl.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var cl = MakeCl();
        cl.Update("BCA Jakarta Baru", "021-6666", null, "Jane", GlobalLeasingId, "admin");
        cl.Deactivate("admin");

        var replayed = Domain.CabangLeasing.CabangLeasing.Reconstitute(cl.DomainEvents);

        Assert.Equal(cl.Id, replayed.Id);
        Assert.Equal(cl.Code, replayed.Code);
        Assert.Equal(cl.Name, replayed.Name);
        Assert.Equal(cl.IsActive, replayed.IsActive);
        Assert.Equal(cl.DealerId, replayed.DealerId);
        Assert.Equal(cl.GlobalLeasingId, replayed.GlobalLeasingId);
    }
}
