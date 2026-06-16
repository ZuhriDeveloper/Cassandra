using Cassandra.Domain.Common;
using Cassandra.Domain.DaftarHargaLeasing;
using Cassandra.Domain.DaftarHargaLeasing.Events;

namespace Cassandra.Tests.DaftarHargaLeasing;

public class DaftarHargaLeasingTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid GlobalLeasingId = Guid.NewGuid();
    private static readonly Guid GrupTenorId = Guid.NewGuid();

    private static Domain.DaftarHargaLeasing.DaftarHargaLeasing MakeDhl(
        string name = "DHL 2024",
        Guid? globalLeasingId = null,
        Guid? grupTenorId = null) =>
        Domain.DaftarHargaLeasing.DaftarHargaLeasing.Create(
            name, globalLeasingId ?? GlobalLeasingId, grupTenorId ?? GrupTenorId, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var dhl = MakeDhl();

        Assert.Single(dhl.DomainEvents);
        var evt = Assert.IsType<DaftarHargaLeasingCreated>(dhl.DomainEvents[0]);

        Assert.Equal("DHL 2024", evt.Name);
        Assert.Equal(GlobalLeasingId, evt.GlobalLeasingId);
        Assert.Equal(GrupTenorId, evt.GrupTenorId);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("DHL 2024", dhl.Name);
        Assert.True(dhl.IsActive);
        Assert.Empty(dhl.Items);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeDhl(name: name));
    }

    [Fact]
    public void Create_ThrowsWhen_GlobalLeasingIdEmpty()
    {
        Assert.Throws<DomainException>(() => MakeDhl(globalLeasingId: Guid.Empty));
    }

    [Fact]
    public void Create_ThrowsWhen_GrupTenorIdEmpty()
    {
        Assert.Throws<DomainException>(() => MakeDhl(grupTenorId: Guid.Empty));
    }

    [Fact]
    public void SetItems_SetsItemsOnAggregate()
    {
        var dhl = MakeDhl();
        dhl.ClearDomainEvents();

        var items = new List<DaftarHargaLeasingItem>
        {
            new(Guid.NewGuid(), 100_000m, 50_000m, 25_000m),
            new(Guid.NewGuid(), 200_000m, 0m, 10_000m)
        };
        dhl.SetItems(items, "admin");

        Assert.IsType<DaftarHargaLeasingItemsSet>(Assert.Single(dhl.DomainEvents));
        Assert.Equal(2, dhl.Items.Count);
        Assert.Equal(100_000m, dhl.Items[0].Subsidi);
    }

    [Fact]
    public void Update_RaisesEvent_WhenDataChanges()
    {
        var dhl = MakeDhl();
        dhl.ClearDomainEvents();

        dhl.Update("DHL 2025", GlobalLeasingId, GrupTenorId, "admin");

        Assert.IsType<DaftarHargaLeasingUpdated>(Assert.Single(dhl.DomainEvents));
        Assert.Equal("DHL 2025", dhl.Name);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var dhl = MakeDhl();
        dhl.ClearDomainEvents();

        dhl.Deactivate("admin");

        Assert.False(dhl.IsActive);
        Assert.IsType<DaftarHargaLeasingDeactivated>(Assert.Single(dhl.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresStateWithItems()
    {
        var dhl = MakeDhl();
        var items = new List<DaftarHargaLeasingItem> { new(Guid.NewGuid(), 100_000m, 50_000m, 25_000m) };
        dhl.SetItems(items, "admin");
        dhl.Deactivate("admin");

        var replayed = Domain.DaftarHargaLeasing.DaftarHargaLeasing.Reconstitute(dhl.DomainEvents);

        Assert.Equal(dhl.Id, replayed.Id);
        Assert.Equal(dhl.Name, replayed.Name);
        Assert.Equal(dhl.IsActive, replayed.IsActive);
        Assert.Single(replayed.Items);
        Assert.Equal(100_000m, replayed.Items[0].Subsidi);
    }
}
