using Cassandra.Domain.Common;
using Cassandra.Domain.GlobalLeasing;
using Cassandra.Domain.GlobalLeasing.Events;

namespace Cassandra.Tests.GlobalLeasing;

public class GlobalLeasingTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.GlobalLeasing.GlobalLeasing MakeGl(
        string code = "BCA",
        string name = "BCA Finance",
        string phone = "021-5555",
        string? fax = null,
        string contact = "John",
        string address = "Jakarta") =>
        Domain.GlobalLeasing.GlobalLeasing.Create(code, name, phone, fax, contact, address, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var gl = MakeGl();

        Assert.Single(gl.DomainEvents);
        var evt = Assert.IsType<GlobalLeasingCreated>(gl.DomainEvents[0]);

        Assert.Equal("BCA", evt.Code);
        Assert.Equal("BCA Finance", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("BCA", gl.Code);
        Assert.Equal("BCA Finance", gl.Name);
        Assert.True(gl.IsActive);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var gl = MakeGl(code: "bca");
        Assert.Equal("BCA", gl.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeGl(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeGl(name: name));
    }

    [Fact]
    public void Update_RaisesEvent_WhenDataChanges()
    {
        var gl = MakeGl();
        gl.ClearDomainEvents();

        gl.Update("BCA Finance Baru", "021-6666", null, "Jane", "Surabaya", "admin");

        Assert.IsType<GlobalLeasingUpdated>(Assert.Single(gl.DomainEvents));
        Assert.Equal("BCA Finance Baru", gl.Name);
        Assert.Equal("021-6666", gl.Phone);
    }

    [Fact]
    public void Update_IsNoop_WhenAllSame()
    {
        var gl = MakeGl();
        gl.ClearDomainEvents();

        gl.Update("BCA Finance", "021-5555", null, "John", "Jakarta", "admin");

        Assert.Empty(gl.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var gl = MakeGl();
        gl.ClearDomainEvents();

        gl.Deactivate("admin");

        Assert.False(gl.IsActive);
        Assert.IsType<GlobalLeasingDeactivated>(Assert.Single(gl.DomainEvents));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var gl = MakeGl();
        gl.Deactivate("admin");
        gl.ClearDomainEvents();

        gl.Activate("admin");

        Assert.True(gl.IsActive);
        Assert.IsType<GlobalLeasingActivated>(Assert.Single(gl.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var gl = MakeGl();
        gl.Update("BCA Finance Baru", "021-6666", null, "Jane", "Surabaya", "admin");
        gl.Deactivate("admin");

        var replayed = Domain.GlobalLeasing.GlobalLeasing.Reconstitute(gl.DomainEvents);

        Assert.Equal(gl.Id, replayed.Id);
        Assert.Equal(gl.Code, replayed.Code);
        Assert.Equal(gl.Name, replayed.Name);
        Assert.Equal(gl.IsActive, replayed.IsActive);
        Assert.Equal(gl.DealerId, replayed.DealerId);
    }
}
