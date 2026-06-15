using Cassandra.Domain.Common;
using Cassandra.Domain.Kelengkapan;
using Cassandra.Domain.Kelengkapan.Events;

namespace Cassandra.Tests.Kelengkapan;

public class KelengkapanTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.Kelengkapan.Kelengkapan MakeKelengkapan(string name = "Helm") =>
        Domain.Kelengkapan.Kelengkapan.Create(name, "admin", DealerId);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var k = MakeKelengkapan();

        Assert.Single(k.DomainEvents);
        var evt = Assert.IsType<KelengkapanCreated>(k.DomainEvents[0]);

        Assert.Equal("Helm", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("Helm", k.Name);
        Assert.True(k.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeKelengkapan(name: name));
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_RaisesEvent_WhenNameChanges()
    {
        var k = MakeKelengkapan();
        k.ClearDomainEvents();

        k.Update("Jaket", "admin");

        var evt = Assert.IsType<KelengkapanUpdated>(Assert.Single(k.DomainEvents));
        Assert.Equal("Jaket", evt.Name);
        Assert.Equal("Jaket", k.Name);
    }

    [Fact]
    public void Update_IsNoop_WhenNameSame()
    {
        var k = MakeKelengkapan();
        k.ClearDomainEvents();

        k.Update("Helm", "admin");

        Assert.Empty(k.DomainEvents);
    }

    // ── Activate / Deactivate ─────────────────────────────────────────────────

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var k = MakeKelengkapan();
        k.ClearDomainEvents();

        k.Deactivate("admin");

        Assert.False(k.IsActive);
        Assert.IsType<KelengkapanDeactivated>(Assert.Single(k.DomainEvents));
    }

    [Fact]
    public void Deactivate_ThrowsWhen_AlreadyInactive()
    {
        var k = MakeKelengkapan();
        k.Deactivate("admin");

        Assert.Throws<DomainException>(() => k.Deactivate("admin"));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var k = MakeKelengkapan();
        k.Deactivate("admin");
        k.ClearDomainEvents();

        k.Activate("admin");

        Assert.True(k.IsActive);
        Assert.IsType<KelengkapanActivated>(Assert.Single(k.DomainEvents));
    }

    [Fact]
    public void Activate_ThrowsWhen_AlreadyActive()
    {
        var k = MakeKelengkapan();

        Assert.Throws<DomainException>(() => k.Activate("admin"));
    }

    // ── Reconstitute ──────────────────────────────────────────────────────────

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var k = MakeKelengkapan();
        k.Update("Jaket", "admin");
        k.Deactivate("admin");

        var replayed = Domain.Kelengkapan.Kelengkapan.Reconstitute(k.DomainEvents);

        Assert.Equal(k.Id, replayed.Id);
        Assert.Equal(k.Name, replayed.Name);
        Assert.Equal(k.IsActive, replayed.IsActive);
        Assert.Equal(k.DealerId, replayed.DealerId);
    }
}
