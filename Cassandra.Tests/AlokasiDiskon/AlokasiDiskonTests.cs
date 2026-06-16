using Cassandra.Domain.AlokasiDiskon;
using Cassandra.Domain.AlokasiDiskon.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.AlokasiDiskon;

public class AlokasiDiskonTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid KaryawanId = Guid.NewGuid();

    private static Domain.AlokasiDiskon.AlokasiDiskon MakeAd(
        Guid? karyawanId = null,
        string discountLevel = "GOLD") =>
        Domain.AlokasiDiskon.AlokasiDiskon.Create(
            karyawanId ?? KaryawanId, discountLevel, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var ad = MakeAd();

        Assert.Single(ad.DomainEvents);
        var evt = Assert.IsType<AlokasiDiskonCreated>(ad.DomainEvents[0]);

        Assert.Equal(KaryawanId, evt.KaryawanId);
        Assert.Equal("GOLD", evt.DiscountLevel);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("GOLD", ad.DiscountLevel);
        Assert.Equal(KaryawanId, ad.KaryawanId);
        Assert.True(ad.IsActive);
    }

    [Fact]
    public void Create_ThrowsWhen_KaryawanIdEmpty()
    {
        Assert.Throws<DomainException>(() => MakeAd(karyawanId: Guid.Empty));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_DiscountLevelEmpty(string level)
    {
        Assert.Throws<DomainException>(() => MakeAd(discountLevel: level));
    }

    [Fact]
    public void Update_RaisesEvent_WhenLevelChanges()
    {
        var ad = MakeAd();
        ad.ClearDomainEvents();

        ad.Update("PLATINUM", "admin");

        Assert.IsType<AlokasiDiskonUpdated>(Assert.Single(ad.DomainEvents));
        Assert.Equal("PLATINUM", ad.DiscountLevel);
    }

    [Fact]
    public void Update_IsNoop_WhenLevelSame()
    {
        var ad = MakeAd();
        ad.ClearDomainEvents();

        ad.Update("GOLD", "admin");

        Assert.Empty(ad.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var ad = MakeAd();
        ad.ClearDomainEvents();

        ad.Deactivate("admin");

        Assert.False(ad.IsActive);
        Assert.IsType<AlokasiDiskonDeactivated>(Assert.Single(ad.DomainEvents));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var ad = MakeAd();
        ad.Deactivate("admin");
        ad.ClearDomainEvents();

        ad.Activate("admin");

        Assert.True(ad.IsActive);
        Assert.IsType<AlokasiDiskonActivated>(Assert.Single(ad.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var ad = MakeAd();
        ad.Update("PLATINUM", "admin");
        ad.Deactivate("admin");

        var replayed = Domain.AlokasiDiskon.AlokasiDiskon.Reconstitute(ad.DomainEvents);

        Assert.Equal(ad.Id, replayed.Id);
        Assert.Equal(ad.KaryawanId, replayed.KaryawanId);
        Assert.Equal(ad.DiscountLevel, replayed.DiscountLevel);
        Assert.Equal(ad.IsActive, replayed.IsActive);
        Assert.Equal(ad.DealerId, replayed.DealerId);
    }
}
