using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTipeMotor;
using Cassandra.Domain.GrupTipeMotor.Events;

namespace Cassandra.Tests.GrupTipeMotor;

public class GrupTipeMotorTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.GrupTipeMotor.GrupTipeMotor MakeGrup(string code = "SPORT") =>
        Domain.GrupTipeMotor.GrupTipeMotor.Create(code, "admin", DealerId);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var grup = MakeGrup();

        Assert.Single(grup.DomainEvents);
        var evt = Assert.IsType<GrupTipeMotorCreated>(grup.DomainEvents[0]);

        Assert.Equal("SPORT", evt.Code);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("SPORT", grup.Code);
        Assert.True(grup.IsActive);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var grup = MakeGrup(code: "sport");
        Assert.Equal("SPORT", grup.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeGrup(code: code));
    }

    // ── Activate / Deactivate ─────────────────────────────────────────────────

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var grup = MakeGrup();
        grup.ClearDomainEvents();

        grup.Deactivate("admin");

        Assert.False(grup.IsActive);
        Assert.IsType<GrupTipeMotorDeactivated>(Assert.Single(grup.DomainEvents));
    }

    [Fact]
    public void Deactivate_ThrowsWhen_AlreadyInactive()
    {
        var grup = MakeGrup();
        grup.Deactivate("admin");

        Assert.Throws<DomainException>(() => grup.Deactivate("admin"));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var grup = MakeGrup();
        grup.Deactivate("admin");
        grup.ClearDomainEvents();

        grup.Activate("admin");

        Assert.True(grup.IsActive);
        Assert.IsType<GrupTipeMotorActivated>(Assert.Single(grup.DomainEvents));
    }

    [Fact]
    public void Activate_ThrowsWhen_AlreadyActive()
    {
        var grup = MakeGrup();

        Assert.Throws<DomainException>(() => grup.Activate("admin"));
    }

    // ── Reconstitute ──────────────────────────────────────────────────────────

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var grup = MakeGrup();
        grup.Deactivate("admin");

        var replayed = Domain.GrupTipeMotor.GrupTipeMotor.Reconstitute(grup.DomainEvents);

        Assert.Equal(grup.Id, replayed.Id);
        Assert.Equal(grup.Code, replayed.Code);
        Assert.Equal(grup.IsActive, replayed.IsActive);
        Assert.Equal(grup.DealerId, replayed.DealerId);
    }
}
