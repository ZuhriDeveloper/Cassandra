using Cassandra.Domain.Common;
using Cassandra.Domain.Ledger;
using Cassandra.Domain.Ledger.Events;

namespace Cassandra.Tests.Ledger;

public class LedgerTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.Ledger.Ledger MakeLedger(string name = "Kas Besar")
        => Domain.Ledger.Ledger.Create(name, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var ledger = MakeLedger();

        Assert.Single(ledger.DomainEvents);
        var evt = Assert.IsType<LedgerCreated>(ledger.DomainEvents[0]);

        Assert.Equal("Kas Besar", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("Kas Besar", ledger.Name);
        Assert.True(ledger.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeLedger(name));
    }

    [Fact]
    public void Update_RaisesEvent_WhenNameChanges()
    {
        var ledger = MakeLedger();
        ledger.ClearDomainEvents();

        ledger.Update("Kas Kecil", "admin");

        var evt = Assert.IsType<LedgerUpdated>(Assert.Single(ledger.DomainEvents));
        Assert.Equal("Kas Kecil", evt.Name);
        Assert.Equal("Kas Kecil", ledger.Name);
    }

    [Fact]
    public void Update_IsNoop_WhenNameSame()
    {
        var ledger = MakeLedger();
        ledger.ClearDomainEvents();

        ledger.Update("Kas Besar", "admin");

        Assert.Empty(ledger.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var ledger = MakeLedger();
        ledger.ClearDomainEvents();

        ledger.Deactivate("admin");

        Assert.False(ledger.IsActive);
        Assert.IsType<LedgerDeactivated>(Assert.Single(ledger.DomainEvents));
    }

    [Fact]
    public void Deactivate_ThrowsWhen_AlreadyInactive()
    {
        var ledger = MakeLedger();
        ledger.Deactivate("admin");

        Assert.Throws<DomainException>(() => ledger.Deactivate("admin"));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var ledger = MakeLedger();
        ledger.Deactivate("admin");
        ledger.ClearDomainEvents();

        ledger.Activate("admin");

        Assert.True(ledger.IsActive);
        Assert.IsType<LedgerActivated>(Assert.Single(ledger.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var ledger = MakeLedger();
        ledger.Update("Kas Kecil", "admin");
        ledger.Deactivate("admin");

        var replayed = Domain.Ledger.Ledger.Reconstitute(ledger.DomainEvents);

        Assert.Equal(ledger.Id, replayed.Id);
        Assert.Equal(ledger.Name, replayed.Name);
        Assert.Equal(ledger.IsActive, replayed.IsActive);
        Assert.Equal(ledger.DealerId, replayed.DealerId);
    }
}
