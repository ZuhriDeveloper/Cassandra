using Cassandra.Domain.Common;
using Cassandra.Domain.Warna;
using Cassandra.Domain.Warna.Events;

namespace Cassandra.Tests.Warna;

public class WarnaTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.Warna.Warna MakeWarna(
        string code = "MR",
        string name = "Merah") =>
        Domain.Warna.Warna.Create(code, name, "admin", DealerId);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var warna = MakeWarna();

        Assert.Single(warna.DomainEvents);
        var evt = Assert.IsType<WarnaCreated>(warna.DomainEvents[0]);

        Assert.Equal("MR", evt.Code);
        Assert.Equal("Merah", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("MR", warna.Code);
        Assert.Equal("Merah", warna.Name);
        Assert.True(warna.IsActive);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var warna = MakeWarna(code: "mr");
        Assert.Equal("MR", warna.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeWarna(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeWarna(name: name));
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_RaisesEvent_WhenNameChanges()
    {
        var warna = MakeWarna();
        warna.ClearDomainEvents();

        warna.Update("Biru", "admin");

        var evt = Assert.IsType<WarnaUpdated>(Assert.Single(warna.DomainEvents));
        Assert.Equal("Biru", evt.Name);
        Assert.Equal("Biru", warna.Name);
    }

    [Fact]
    public void Update_IsNoop_WhenNameSame()
    {
        var warna = MakeWarna();
        warna.ClearDomainEvents();

        warna.Update("Merah", "admin");

        Assert.Empty(warna.DomainEvents);
    }

    // ── Activate / Deactivate ─────────────────────────────────────────────────

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var warna = MakeWarna();
        warna.ClearDomainEvents();

        warna.Deactivate("admin");

        Assert.False(warna.IsActive);
        Assert.IsType<WarnaDeactivated>(Assert.Single(warna.DomainEvents));
    }

    [Fact]
    public void Deactivate_ThrowsWhen_AlreadyInactive()
    {
        var warna = MakeWarna();
        warna.Deactivate("admin");

        Assert.Throws<DomainException>(() => warna.Deactivate("admin"));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var warna = MakeWarna();
        warna.Deactivate("admin");
        warna.ClearDomainEvents();

        warna.Activate("admin");

        Assert.True(warna.IsActive);
        Assert.IsType<WarnaActivated>(Assert.Single(warna.DomainEvents));
    }

    [Fact]
    public void Activate_ThrowsWhen_AlreadyActive()
    {
        var warna = MakeWarna();

        Assert.Throws<DomainException>(() => warna.Activate("admin"));
    }

    // ── Reconstitute ──────────────────────────────────────────────────────────

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var warna = MakeWarna();
        warna.Update("Biru", "admin");
        warna.Deactivate("admin");

        var replayed = Domain.Warna.Warna.Reconstitute(warna.DomainEvents);

        Assert.Equal(warna.Id, replayed.Id);
        Assert.Equal(warna.Code, replayed.Code);
        Assert.Equal(warna.Name, replayed.Name);
        Assert.Equal(warna.IsActive, replayed.IsActive);
        Assert.Equal(warna.DealerId, replayed.DealerId);
    }
}
