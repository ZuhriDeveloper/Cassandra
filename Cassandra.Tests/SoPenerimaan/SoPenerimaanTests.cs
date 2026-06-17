using Cassandra.Domain.Common;
using Cassandra.Domain.SoPenerimaan;
using Cassandra.Domain.SoPenerimaan.Events;

namespace Cassandra.Tests.SoPenerimaan;

public class SoPenerimaanTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid SoId = Guid.NewGuid();
    private static readonly DateOnly SuratJalanDate = DateOnly.FromDateTime(DateTime.Today);

    private static List<SoPenerimaanItemMotorValue> DefaultMotorItems() =>
    [
        new SoPenerimaanItemMotorValue(Guid.NewGuid(), Guid.NewGuid(), "M001", "R001", Guid.NewGuid(), "2025")
    ];

    private static Domain.SoPenerimaan.SoPenerimaan MakeSoPenerimaan(
        string suratJalanId = "SJ-001",
        List<SoPenerimaanItemMotorValue>? motorItems = null)
    {
        return Domain.SoPenerimaan.SoPenerimaan.Create(
            suratJalanId, SuratJalanDate, SoId,
            motorItems ?? DefaultMotorItems(),
            [],
            "admin", DealerId);
    }

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var sp = MakeSoPenerimaan();

        Assert.Single(sp.DomainEvents);
        var evt = Assert.IsType<SoPenerimaanCreated>(sp.DomainEvents[0]);

        Assert.Equal("SJ-001", evt.SuratJalanId);
        Assert.Equal(SoId, evt.SoId);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Single(sp.MotorItems);
        Assert.Empty(sp.KelengkapanItems);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_SuratJalanIdEmpty(string suratJalanId)
    {
        Assert.Throws<DomainException>(() => MakeSoPenerimaan(suratJalanId: suratJalanId));
    }

    [Fact]
    public void Create_ThrowsWhen_MotorItemsEmpty()
    {
        Assert.Throws<DomainException>(() => MakeSoPenerimaan(motorItems: []));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var sp = MakeSoPenerimaan();

        var replayed = Domain.SoPenerimaan.SoPenerimaan.Reconstitute(sp.DomainEvents);

        Assert.Equal(sp.Id, replayed.Id);
        Assert.Equal(sp.SuratJalanId, replayed.SuratJalanId);
        Assert.Equal(sp.SoId, replayed.SoId);
        Assert.Equal(sp.DealerId, replayed.DealerId);
        Assert.Single(replayed.MotorItems);
    }
}
