using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers;
using Cassandra.Domain.Dealers.Events;

namespace Cassandra.Tests.Dealers;

public class DealerTests
{
    [Fact]
    public void Register_raises_DealerRegistered_and_sets_state()
    {
        var dealer = Dealer.Register("  Dealer Pusat  ", "  D1  ", "superadmin@cassandra.local");

        Assert.Equal("Dealer Pusat", dealer.Name);
        Assert.Equal("D1", dealer.Code);
        Assert.True(dealer.IsActive);
        Assert.Equal(1, dealer.Version);

        var evt = Assert.IsType<DealerRegistered>(Assert.Single(dealer.DomainEvents));
        Assert.Equal("Dealer Pusat", evt.Name);
        Assert.Equal("D1", evt.Code);
        Assert.Equal("superadmin@cassandra.local", evt.RegisteredBy);
        Assert.Equal(dealer.Id, evt.DealerId);
    }

    [Theory]
    [InlineData("", "D1")]
    [InlineData("   ", "D1")]
    [InlineData("Dealer", "")]
    [InlineData("Dealer", "   ")]
    public void Register_rejects_empty_name_or_code(string name, string code)
    {
        Assert.Throws<DomainException>(() => Dealer.Register(name, code, "admin"));
    }

    [Fact]
    public void Rename_changes_name_and_raises_event()
    {
        var dealer = Dealer.Register("Old Name", "D1", "admin");
        dealer.ClearDomainEvents();

        dealer.Rename("New Name", "admin");

        Assert.Equal("New Name", dealer.Name);
        Assert.IsType<DealerRenamed>(Assert.Single(dealer.DomainEvents));
    }

    [Fact]
    public void Rename_to_same_name_is_a_noop()
    {
        var dealer = Dealer.Register("Same", "D1", "admin");
        dealer.ClearDomainEvents();

        dealer.Rename("Same", "admin");

        Assert.Empty(dealer.DomainEvents);
    }

    [Fact]
    public void Deactivate_then_activate_toggles_status_with_guards()
    {
        var dealer = Dealer.Register("Dealer", "D1", "admin");

        dealer.Deactivate("admin");
        Assert.False(dealer.IsActive);
        // Already inactive
        Assert.Throws<DomainException>(() => dealer.Deactivate("admin"));

        dealer.Activate("admin");
        Assert.True(dealer.IsActive);
        // Already active
        Assert.Throws<DomainException>(() => dealer.Activate("admin"));
    }

    [Fact]
    public void Reconstitute_replays_events_to_same_state_and_version()
    {
        var original = Dealer.Register("Dealer", "D1", "admin");
        original.Rename("Dealer Renamed", "admin");
        original.Deactivate("admin");

        IEnumerable<IDomainEvent> events = original.DomainEvents.ToList();
        var rebuilt = Dealer.Reconstitute(events);

        Assert.Equal(original.Id, rebuilt.Id);
        Assert.Equal("Dealer Renamed", rebuilt.Name);
        Assert.Equal("D1", rebuilt.Code);
        Assert.False(rebuilt.IsActive);
        Assert.Equal(original.Version, rebuilt.Version);
        // Reconstituted aggregate has no uncommitted events
        Assert.Empty(rebuilt.DomainEvents);
    }
}
