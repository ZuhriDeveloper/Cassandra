using Cassandra.Domain.Common;
using Cassandra.Domain.TipeMotor;
using Cassandra.Domain.TipeMotor.Events;

namespace Cassandra.Tests.TipeMotor;

public class TipeMotorTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid GrupId = Guid.NewGuid();

    private static Domain.TipeMotor.TipeMotor MakeTipe(
        string code = "CB150",
        Guid? grupId = null,
        string shortName = "CB150R") =>
        Domain.TipeMotor.TipeMotor.Create(
            code,
            grupId ?? GrupId,
            shortName,
            "PC001",
            "WMS001",
            "AHM001",
            "NXXX",
            "CXXX",
            50_000_000m,
            1_000_000m,
            900_000m,
            500_000m,
            450_000m,
            "admin",
            DealerId);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var tipe = MakeTipe();

        Assert.Single(tipe.DomainEvents);
        var evt = Assert.IsType<TipeMotorCreated>(tipe.DomainEvents[0]);

        Assert.Equal("CB150", evt.Code);
        Assert.Equal(GrupId, evt.GrupTipeMotorId);
        Assert.Equal("CB150R", evt.ShortName);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(50_000_000m, evt.NettPrice);
        Assert.Equal("CB150", tipe.Code);
        Assert.True(tipe.IsActive);
        Assert.Empty(tipe.WarnaIds);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var tipe = MakeTipe(code: "cb150");
        Assert.Equal("CB150", tipe.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeTipe(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_ShortNameEmpty(string shortName)
    {
        Assert.Throws<DomainException>(() => MakeTipe(shortName: shortName));
    }

    [Fact]
    public void Create_ThrowsWhen_GrupTipeMotorIdEmpty()
    {
        Assert.Throws<DomainException>(() => MakeTipe(grupId: Guid.Empty));
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_RaisesEvent_WhenDataChanges()
    {
        var tipe = MakeTipe();
        tipe.ClearDomainEvents();

        var newGrupId = Guid.NewGuid();
        tipe.Update(newGrupId, "CB150R-New", "PC002", "WMS002", "AHM002",
            "NYYY", "CYYY", 55_000_000m, 1_100_000m, 950_000m, 550_000m, 500_000m, "admin");

        var evt = Assert.IsType<TipeMotorUpdated>(Assert.Single(tipe.DomainEvents));
        Assert.Equal(newGrupId, evt.GrupTipeMotorId);
        Assert.Equal("CB150R-New", evt.ShortName);
        Assert.Equal(55_000_000m, evt.NettPrice);
    }

    [Fact]
    public void Update_IsNoop_WhenAllSame()
    {
        var tipe = MakeTipe();
        tipe.ClearDomainEvents();

        tipe.Update(GrupId, "CB150R", "PC001", "WMS001", "AHM001",
            "NXXX", "CXXX", 50_000_000m, 1_000_000m, 900_000m, 500_000m, 450_000m, "admin");

        Assert.Empty(tipe.DomainEvents);
    }

    // ── SetColors ─────────────────────────────────────────────────────────────

    [Fact]
    public void SetColors_RaisesEvent()
    {
        var tipe = MakeTipe();
        tipe.ClearDomainEvents();

        var warnaId1 = Guid.NewGuid();
        var warnaId2 = Guid.NewGuid();
        tipe.SetColors([warnaId1, warnaId2], "admin");

        var evt = Assert.IsType<TipeMotorColorsSet>(Assert.Single(tipe.DomainEvents));
        Assert.Equal(2, evt.WarnaIds.Count);
        Assert.Contains(warnaId1, evt.WarnaIds);
        Assert.Contains(warnaId2, evt.WarnaIds);
        Assert.Equal(2, tipe.WarnaIds.Count);
    }

    // ── Activate / Deactivate ─────────────────────────────────────────────────

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var tipe = MakeTipe();
        tipe.ClearDomainEvents();

        tipe.Deactivate("admin");

        Assert.False(tipe.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var tipe = MakeTipe();
        tipe.Deactivate("admin");
        tipe.ClearDomainEvents();

        tipe.Activate("admin");

        Assert.True(tipe.IsActive);
    }

    // ── Reconstitute ──────────────────────────────────────────────────────────

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var tipe = MakeTipe();
        var warnaId = Guid.NewGuid();
        tipe.SetColors([warnaId], "admin");
        tipe.Deactivate("admin");

        var replayed = Domain.TipeMotor.TipeMotor.Reconstitute(tipe.DomainEvents);

        Assert.Equal(tipe.Id, replayed.Id);
        Assert.Equal(tipe.Code, replayed.Code);
        Assert.Equal(tipe.GrupTipeMotorId, replayed.GrupTipeMotorId);
        Assert.Equal(tipe.ShortName, replayed.ShortName);
        Assert.Equal(tipe.IsActive, replayed.IsActive);
        Assert.Equal(tipe.WarnaIds.Count, replayed.WarnaIds.Count);
    }
}
