using Cassandra.Domain.Common;
using Cassandra.Domain.Jabatan;
using Cassandra.Domain.Jabatan.Events;

namespace Cassandra.Tests.Jabatan;

public class JabatanTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    [Fact]
    public void Create_raises_JabatanCreated_and_sets_state()
    {
        var jabatan = Domain.Jabatan.Jabatan.Create("  Kepala Mekanik  ", "Bertanggung jawab atas mekanik", "admin@cassandra.local", DealerId);

        Assert.Equal("Kepala Mekanik", jabatan.Name);
        Assert.Equal("Bertanggung jawab atas mekanik", jabatan.Description);
        Assert.Equal(DealerId, jabatan.DealerId);
        Assert.True(jabatan.IsActive);
        Assert.Equal(1, jabatan.Version);

        var evt = Assert.IsType<JabatanCreated>(Assert.Single(jabatan.DomainEvents));
        Assert.Equal("Kepala Mekanik", evt.Name);
        Assert.Equal(jabatan.Id, evt.JabatanId);
        Assert.Equal(DealerId, evt.DealerId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_empty_name(string name)
    {
        Assert.Throws<DomainException>(() =>
            Domain.Jabatan.Jabatan.Create(name, "desc", "admin", DealerId));
    }

    [Fact]
    public void Update_changes_name_and_description_and_raises_event()
    {
        var jabatan = Domain.Jabatan.Jabatan.Create("Old Name", "Old Desc", "admin", DealerId);
        jabatan.ClearDomainEvents();

        jabatan.Update("New Name", "New Desc", "admin");

        Assert.Equal("New Name", jabatan.Name);
        Assert.Equal("New Desc", jabatan.Description);
        Assert.IsType<JabatanUpdated>(Assert.Single(jabatan.DomainEvents));
    }

    [Fact]
    public void Update_to_same_values_is_a_noop()
    {
        var jabatan = Domain.Jabatan.Jabatan.Create("Same", "Desc", "admin", DealerId);
        jabatan.ClearDomainEvents();

        jabatan.Update("Same", "Desc", "admin");

        Assert.Empty(jabatan.DomainEvents);
    }

    [Fact]
    public void Deactivate_then_activate_toggles_status_with_guards()
    {
        var jabatan = Domain.Jabatan.Jabatan.Create("Jabatan", "desc", "admin", DealerId);

        jabatan.Deactivate("admin");
        Assert.False(jabatan.IsActive);
        Assert.Throws<DomainException>(() => jabatan.Deactivate("admin"));

        jabatan.Activate("admin");
        Assert.True(jabatan.IsActive);
        Assert.Throws<DomainException>(() => jabatan.Activate("admin"));
    }

    [Fact]
    public void Reconstitute_replays_events_to_same_state_and_version()
    {
        var original = Domain.Jabatan.Jabatan.Create("Jabatan", "Desc", "admin", DealerId);
        original.Update("Jabatan Updated", "New Desc", "admin");
        original.Deactivate("admin");

        IEnumerable<IDomainEvent> events = original.DomainEvents.ToList();
        var rebuilt = Domain.Jabatan.Jabatan.Reconstitute(events);

        Assert.Equal(original.Id, rebuilt.Id);
        Assert.Equal("Jabatan Updated", rebuilt.Name);
        Assert.Equal("New Desc", rebuilt.Description);
        Assert.Equal(DealerId, rebuilt.DealerId);
        Assert.False(rebuilt.IsActive);
        Assert.Equal(original.Version, rebuilt.Version);
        Assert.Empty(rebuilt.DomainEvents);
    }
}
