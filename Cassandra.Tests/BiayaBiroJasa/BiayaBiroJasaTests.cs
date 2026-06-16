using Cassandra.Domain.BiayaBiroJasa;
using Cassandra.Domain.BiayaBiroJasa.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.BiayaBiroJasa;

public class BiayaBiroJasaTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid SamsatId = Guid.NewGuid();
    private static readonly Guid BiroId = Guid.NewGuid();

    private static Domain.BiayaBiroJasa.BiayaBiroJasa MakeBbj()
        => Domain.BiayaBiroJasa.BiayaBiroJasa.Create(SamsatId, BiroId, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var bbj = MakeBbj();

        Assert.Single(bbj.DomainEvents);
        var evt = Assert.IsType<BiayaBiroJasaCreated>(bbj.DomainEvents[0]);

        Assert.Equal(SamsatId, evt.SamsatId);
        Assert.Equal(BiroId, evt.BiroId);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(SamsatId, bbj.SamsatId);
        Assert.Equal(BiroId, bbj.BiroId);
        Assert.True(bbj.IsActive);
        Assert.Empty(bbj.Items);
    }

    [Fact]
    public void Create_ThrowsWhen_SamsatIdEmpty()
    {
        Assert.Throws<DomainException>(() =>
            Domain.BiayaBiroJasa.BiayaBiroJasa.Create(Guid.Empty, BiroId, "admin", DealerId));
    }

    [Fact]
    public void Create_ThrowsWhen_BiroIdEmpty()
    {
        Assert.Throws<DomainException>(() =>
            Domain.BiayaBiroJasa.BiayaBiroJasa.Create(SamsatId, Guid.Empty, "admin", DealerId));
    }

    [Fact]
    public void SetItems_RaisesEvent_AndUpdatesItems()
    {
        var bbj = MakeBbj();
        bbj.ClearDomainEvents();

        var tipeMotorId = Guid.NewGuid();
        var items = new List<BiayaBiroJasaItemValue>
        {
            new(tipeMotorId, 150000m, 50000m)
        };

        bbj.SetItems(items, "admin");

        var evt = Assert.IsType<BiayaBiroJasaItemsSet>(Assert.Single(bbj.DomainEvents));
        Assert.Single(evt.Items);
        Assert.Single(bbj.Items);
        Assert.Equal(150000m, bbj.Items[0].BiayaStnk);
        Assert.Equal(50000m, bbj.Items[0].Notice);
    }

    [Fact]
    public void SetItems_ThrowsWhen_BiayaStnkNegative()
    {
        var bbj = MakeBbj();
        var items = new List<BiayaBiroJasaItemValue> { new(Guid.NewGuid(), -1m, 0m) };

        Assert.Throws<DomainException>(() => bbj.SetItems(items, "admin"));
    }

    [Fact]
    public void SetItems_ThrowsWhen_NoticeNegative()
    {
        var bbj = MakeBbj();
        var items = new List<BiayaBiroJasaItemValue> { new(Guid.NewGuid(), 0m, -1m) };

        Assert.Throws<DomainException>(() => bbj.SetItems(items, "admin"));
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var bbj = MakeBbj();
        bbj.ClearDomainEvents();

        bbj.Deactivate("admin");

        Assert.False(bbj.IsActive);
        Assert.IsType<BiayaBiroJasaDeactivated>(Assert.Single(bbj.DomainEvents));
    }

    [Fact]
    public void Deactivate_ThrowsWhen_AlreadyInactive()
    {
        var bbj = MakeBbj();
        bbj.Deactivate("admin");

        Assert.Throws<DomainException>(() => bbj.Deactivate("admin"));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var bbj = MakeBbj();
        bbj.Deactivate("admin");
        bbj.ClearDomainEvents();

        bbj.Activate("admin");

        Assert.True(bbj.IsActive);
        Assert.IsType<BiayaBiroJasaActivated>(Assert.Single(bbj.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var bbj = MakeBbj();
        var tipeMotorId = Guid.NewGuid();
        bbj.SetItems([new(tipeMotorId, 100000m, 25000m)], "admin");
        bbj.Deactivate("admin");

        var replayed = Domain.BiayaBiroJasa.BiayaBiroJasa.Reconstitute(bbj.DomainEvents);

        Assert.Equal(bbj.Id, replayed.Id);
        Assert.Equal(bbj.SamsatId, replayed.SamsatId);
        Assert.Equal(bbj.BiroId, replayed.BiroId);
        Assert.Equal(bbj.IsActive, replayed.IsActive);
        Assert.Single(replayed.Items);
    }
}
