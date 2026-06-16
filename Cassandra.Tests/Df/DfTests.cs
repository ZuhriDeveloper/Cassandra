using Cassandra.Domain.Df;
using Cassandra.Domain.Df.Events;

namespace Cassandra.Tests.Df;

public class DfTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.Df.Df MakeDf(
        decimal discount = 5m,
        decimal interest = 3m,
        DateOnly? startDate = null) =>
        Domain.Df.Df.Create(discount, interest, startDate ?? new DateOnly(2024, 1, 1), "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var df = MakeDf();

        Assert.Single(df.DomainEvents);
        var evt = Assert.IsType<DfSet>(df.DomainEvents[0]);

        Assert.Equal(5m, evt.Discount);
        Assert.Equal(3m, evt.Interest);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(5m, df.Discount);
        Assert.Equal(3m, df.Interest);
        Assert.Equal(DealerId, df.DealerId);
    }

    [Fact]
    public void Set_RaisesEvent_WithNewValues()
    {
        var df = MakeDf();
        df.ClearDomainEvents();

        df.Set(7m, 4m, new DateOnly(2024, 6, 1), "admin");

        Assert.IsType<DfSet>(Assert.Single(df.DomainEvents));
        Assert.Equal(7m, df.Discount);
        Assert.Equal(4m, df.Interest);
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var df = MakeDf();
        df.Set(7m, 4m, new DateOnly(2024, 6, 1), "admin");

        var replayed = Domain.Df.Df.Reconstitute(df.DomainEvents);

        Assert.Equal(df.Id, replayed.Id);
        Assert.Equal(df.Discount, replayed.Discount);
        Assert.Equal(df.Interest, replayed.Interest);
        Assert.Equal(df.StartDate, replayed.StartDate);
        Assert.Equal(df.DealerId, replayed.DealerId);
    }

    [Fact]
    public void Reconstitute_LastSet_Wins()
    {
        var df = MakeDf(discount: 5m, interest: 3m);
        df.Set(7m, 4m, new DateOnly(2024, 6, 1), "admin");
        df.Set(10m, 2m, new DateOnly(2024, 9, 1), "admin");

        var replayed = Domain.Df.Df.Reconstitute(df.DomainEvents);

        Assert.Equal(10m, replayed.Discount);
        Assert.Equal(2m, replayed.Interest);
    }
}
