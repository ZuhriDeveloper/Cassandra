using Cassandra.Domain.Common;
using Cassandra.Domain.PelanggaranWilayah;
using Cassandra.Domain.PelanggaranWilayah.Events;

namespace Cassandra.Tests.PelanggaranWilayah;

public class PelanggaranWilayahTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.PelanggaranWilayah.PelanggaranWilayah MakePw(
        string areaCode = "021",
        decimal extraFee = 500000m)
        => Domain.PelanggaranWilayah.PelanggaranWilayah.Create(areaCode, extraFee, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var pw = MakePw();

        Assert.Single(pw.DomainEvents);
        var evt = Assert.IsType<PelanggaranWilayahCreated>(pw.DomainEvents[0]);

        Assert.Equal("021", evt.AreaCode);
        Assert.Equal(500000m, evt.ExtraFee);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("021", pw.AreaCode);
        Assert.Equal(500000m, pw.ExtraFee);
        Assert.True(pw.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_AreaCodeEmpty(string areaCode)
    {
        Assert.Throws<DomainException>(() => MakePw(areaCode: areaCode));
    }

    [Fact]
    public void Create_ThrowsWhen_ExtraFeeNegative()
    {
        Assert.Throws<DomainException>(() => MakePw(extraFee: -1m));
    }

    [Fact]
    public void Create_AllowsZeroExtraFee()
    {
        var pw = MakePw(extraFee: 0m);
        Assert.Equal(0m, pw.ExtraFee);
    }

    [Fact]
    public void Update_RaisesEvent_WhenExtraFeeChanges()
    {
        var pw = MakePw();
        pw.ClearDomainEvents();

        pw.Update(750000m, "admin");

        var evt = Assert.IsType<PelanggaranWilayahUpdated>(Assert.Single(pw.DomainEvents));
        Assert.Equal(750000m, evt.ExtraFee);
        Assert.Equal(750000m, pw.ExtraFee);
    }

    [Fact]
    public void Update_IsNoop_WhenExtraFeeSame()
    {
        var pw = MakePw();
        pw.ClearDomainEvents();

        pw.Update(500000m, "admin");

        Assert.Empty(pw.DomainEvents);
    }

    [Fact]
    public void Update_ThrowsWhen_ExtraFeeNegative()
    {
        var pw = MakePw();
        Assert.Throws<DomainException>(() => pw.Update(-1m, "admin"));
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var pw = MakePw();
        pw.ClearDomainEvents();

        pw.Deactivate("admin");

        Assert.False(pw.IsActive);
        Assert.IsType<PelanggaranWilayahDeactivated>(Assert.Single(pw.DomainEvents));
    }

    [Fact]
    public void Deactivate_ThrowsWhen_AlreadyInactive()
    {
        var pw = MakePw();
        pw.Deactivate("admin");

        Assert.Throws<DomainException>(() => pw.Deactivate("admin"));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var pw = MakePw();
        pw.Deactivate("admin");
        pw.ClearDomainEvents();

        pw.Activate("admin");

        Assert.True(pw.IsActive);
        Assert.IsType<PelanggaranWilayahActivated>(Assert.Single(pw.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var pw = MakePw();
        pw.Update(750000m, "admin");
        pw.Deactivate("admin");

        var replayed = Domain.PelanggaranWilayah.PelanggaranWilayah.Reconstitute(pw.DomainEvents);

        Assert.Equal(pw.Id, replayed.Id);
        Assert.Equal(pw.AreaCode, replayed.AreaCode);
        Assert.Equal(pw.ExtraFee, replayed.ExtraFee);
        Assert.Equal(pw.IsActive, replayed.IsActive);
        Assert.Equal(pw.DealerId, replayed.DealerId);
    }
}
