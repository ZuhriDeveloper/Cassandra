using Cassandra.Domain.Common;
using Cassandra.Domain.SoRetur;
using Cassandra.Domain.SoRetur.Events;

namespace Cassandra.Tests.SoRetur;

public class SoReturTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid SoId = Guid.NewGuid();

    private static List<SoReturItemValue> DefaultItems() =>
    [
        new SoReturItemValue(Guid.NewGuid(), Guid.NewGuid(), 1, 25_000_000m)
    ];

    private static Domain.SoRetur.SoRetur MakeSoRetur(
        string returNumber = "RET-001",
        List<SoReturItemValue>? items = null)
    {
        return Domain.SoRetur.SoRetur.Create(
            returNumber, SoId,
            DateOnly.FromDateTime(DateTime.Today),
            "Unit cacat dari pabrik",
            items ?? DefaultItems(),
            "admin", DealerId);
    }

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var retur = MakeSoRetur();

        Assert.Single(retur.DomainEvents);
        var evt = Assert.IsType<SoReturCreated>(retur.DomainEvents[0]);

        Assert.Equal("RET-001", evt.ReturNumber);
        Assert.Equal(SoId, evt.SoId);
        Assert.Equal(DealerId, evt.DealerId);
    }

    [Fact]
    public void Create_CalculatesTotalAndPpnCorrectly()
    {
        var items = new List<SoReturItemValue>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), 2, 10_000_000m),
            new(Guid.NewGuid(), Guid.NewGuid(), 1, 5_000_000m),
        };
        var retur = MakeSoRetur(items: items);

        // Total = 2 * 10M + 1 * 5M = 25M
        Assert.Equal(25_000_000m, retur.Total);
        // PPn = 25M * 0.1 = 2.5M
        Assert.Equal(2_500_000m, retur.PPn);
        // TotalAmount = 25M + 2.5M = 27.5M
        Assert.Equal(27_500_000m, retur.TotalAmount);
    }

    [Fact]
    public void Create_NormalisesReturNumberToUppercase()
    {
        var retur = MakeSoRetur(returNumber: "ret-001");
        Assert.Equal("RET-001", retur.ReturNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_ReturNumberEmpty(string returNumber)
    {
        Assert.Throws<DomainException>(() => MakeSoRetur(returNumber: returNumber));
    }

    [Fact]
    public void Create_ThrowsWhen_ItemsEmpty()
    {
        Assert.Throws<DomainException>(() => MakeSoRetur(items: []));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var retur = MakeSoRetur();

        var replayed = Domain.SoRetur.SoRetur.Reconstitute(retur.DomainEvents);

        Assert.Equal(retur.Id, replayed.Id);
        Assert.Equal(retur.ReturNumber, replayed.ReturNumber);
        Assert.Equal(retur.Total, replayed.Total);
        Assert.Equal(retur.PPn, replayed.PPn);
        Assert.Equal(retur.TotalAmount, replayed.TotalAmount);
    }
}
