using Cassandra.Domain.Common;
using Cassandra.Domain.PengirimanMotor;

namespace Cassandra.Tests.PengirimanMotor;

public class PengirimanMotorAggregateTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    [Fact]
    public void Create_SetsInitialState()
    {
        var registrasiId = Guid.NewGuid();
        var driver1      = Guid.NewGuid();

        var pm = Domain.PengirimanMotor.PengirimanMotor.Create(
            registrasiId, "M001", driver1, null,
            DateOnly.FromDateTime(DateTime.Today), "Zona A", "driver1", DealerId);

        Assert.Equal(registrasiId, pm.RegistrasiPenjualanId);
        Assert.Equal("M001", pm.NoMesin);
        Assert.Equal(driver1, pm.Driver1Id);
        Assert.Null(pm.Driver2Id);
        Assert.Equal("Zona A", pm.Zona);
        Assert.Equal(DealerId, pm.DealerId);
        Assert.Equal(1, pm.Version);
    }

    [Fact]
    public void Create_Throws_WhenNoMesinEmpty()
    {
        Assert.Throws<DomainException>(() =>
            Domain.PengirimanMotor.PengirimanMotor.Create(
                Guid.NewGuid(), "", Guid.NewGuid(), null,
                DateOnly.FromDateTime(DateTime.Today), null, "driver", DealerId));
    }

    [Fact]
    public void Create_Throws_WhenDriver1Empty()
    {
        Assert.Throws<DomainException>(() =>
            Domain.PengirimanMotor.PengirimanMotor.Create(
                Guid.NewGuid(), "M001", Guid.Empty, null,
                DateOnly.FromDateTime(DateTime.Today), null, "driver", DealerId));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var registrasiId = Guid.NewGuid();
        var driver1      = Guid.NewGuid();
        var original     = Domain.PengirimanMotor.PengirimanMotor.Create(
            registrasiId, "M001", driver1, null,
            DateOnly.FromDateTime(DateTime.Today), null, "driver", DealerId);

        var reconstituted = Domain.PengirimanMotor.PengirimanMotor.Reconstitute(original.DomainEvents);

        Assert.Equal(original.RegistrasiPenjualanId, reconstituted.RegistrasiPenjualanId);
        Assert.Equal(original.NoMesin, reconstituted.NoMesin);
        Assert.Equal(original.Driver1Id, reconstituted.Driver1Id);
    }
}
