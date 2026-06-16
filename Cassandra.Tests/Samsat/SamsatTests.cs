using Cassandra.Domain.Common;
using Cassandra.Domain.Samsat;
using Cassandra.Domain.Samsat.Events;

namespace Cassandra.Tests.Samsat;

public class SamsatTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.Samsat.Samsat MakeSamsat(string name = "Samsat Jakarta Barat")
        => Domain.Samsat.Samsat.Create(name, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var samsat = MakeSamsat();

        Assert.Single(samsat.DomainEvents);
        var evt = Assert.IsType<SamsatCreated>(samsat.DomainEvents[0]);

        Assert.Equal("Samsat Jakarta Barat", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("Samsat Jakarta Barat", samsat.Name);
        Assert.True(samsat.IsActive);
        Assert.Empty(samsat.Cities);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeSamsat(name));
    }

    [Fact]
    public void Update_RaisesEvent_WhenNameChanges()
    {
        var samsat = MakeSamsat();
        samsat.ClearDomainEvents();

        samsat.Update("Samsat Jakarta Timur", "admin");

        var evt = Assert.IsType<SamsatUpdated>(Assert.Single(samsat.DomainEvents));
        Assert.Equal("Samsat Jakarta Timur", evt.Name);
        Assert.Equal("Samsat Jakarta Timur", samsat.Name);
    }

    [Fact]
    public void Update_IsNoop_WhenNameSame()
    {
        var samsat = MakeSamsat();
        samsat.ClearDomainEvents();

        samsat.Update("Samsat Jakarta Barat", "admin");

        Assert.Empty(samsat.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var samsat = MakeSamsat();
        samsat.ClearDomainEvents();

        samsat.Deactivate("admin");

        Assert.False(samsat.IsActive);
        Assert.IsType<SamsatDeactivated>(Assert.Single(samsat.DomainEvents));
    }

    [Fact]
    public void Deactivate_ThrowsWhen_AlreadyInactive()
    {
        var samsat = MakeSamsat();
        samsat.Deactivate("admin");

        Assert.Throws<DomainException>(() => samsat.Deactivate("admin"));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var samsat = MakeSamsat();
        samsat.Deactivate("admin");
        samsat.ClearDomainEvents();

        samsat.Activate("admin");

        Assert.True(samsat.IsActive);
        Assert.IsType<SamsatActivated>(Assert.Single(samsat.DomainEvents));
    }

    [Fact]
    public void Activate_ThrowsWhen_AlreadyActive()
    {
        var samsat = MakeSamsat();
        Assert.Throws<DomainException>(() => samsat.Activate("admin"));
    }

    [Fact]
    public void SetCities_RaisesEvent_AndUpdatesCities()
    {
        var samsat = MakeSamsat();
        samsat.ClearDomainEvents();

        var cities = new List<string> { "Jakarta Barat", "Tangerang" };
        samsat.SetCities(cities, "admin");

        var evt = Assert.IsType<SamsatCitiesSet>(Assert.Single(samsat.DomainEvents));
        Assert.Equal(2, evt.Cities.Count);
        Assert.Equal(2, samsat.Cities.Count);
        Assert.Contains("Jakarta Barat", samsat.Cities);
        Assert.Contains("Tangerang", samsat.Cities);
    }

    [Fact]
    public void SetCities_WithEmptyList_ClearsCities()
    {
        var samsat = MakeSamsat();
        samsat.SetCities(["Jakarta Barat"], "admin");
        samsat.ClearDomainEvents();

        samsat.SetCities([], "admin");

        Assert.Empty(samsat.Cities);
        Assert.IsType<SamsatCitiesSet>(Assert.Single(samsat.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var samsat = MakeSamsat();
        samsat.Update("Samsat Baru", "admin");
        samsat.SetCities(["Kota A"], "admin");
        samsat.Deactivate("admin");

        var replayed = Domain.Samsat.Samsat.Reconstitute(samsat.DomainEvents);

        Assert.Equal(samsat.Id, replayed.Id);
        Assert.Equal(samsat.Name, replayed.Name);
        Assert.Equal(samsat.IsActive, replayed.IsActive);
        Assert.Equal(samsat.DealerId, replayed.DealerId);
        Assert.Single(replayed.Cities);
    }
}
