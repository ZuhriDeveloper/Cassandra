using Cassandra.Domain.ApTransaction;
using Cassandra.Domain.ApTransaction.Events;
using Cassandra.Domain.ArTransaction;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.ApTransaction;

public class ApTransactionTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly ApTransactionId Id = ApTransactionId.New();
    private static readonly Guid StnkId = Guid.NewGuid();

    private static Domain.ApTransaction.ApTransaction CreateSample(decimal totalAmount = 2_000_000m) =>
        Domain.ApTransaction.ApTransaction.Create(
            Id,
            Domain.ApTransaction.ApTransaction.STNK,
            StnkId,
            "B1234CD",
            totalAmount,
            "admin",
            DealerId);

    [Fact]
    public void Create_ShouldRaiseApTransactionCreated()
    {
        var ap = CreateSample();

        var evt = Assert.IsType<ApTransactionCreated>(Assert.Single(ap.DomainEvents));
        Assert.Equal(Domain.ApTransaction.ApTransaction.STNK, evt.TransactionType);
        Assert.Equal(2_000_000m, evt.TotalAmount);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(StnkId, evt.StnkId);
        Assert.Equal("B1234CD", evt.NoRangka);
    }

    [Fact]
    public void Create_SetsInitialState()
    {
        var ap = CreateSample(500_000m);

        Assert.Equal(500_000m, ap.TotalAmount);
        Assert.Equal(500_000m, ap.RemainingAmount);
        Assert.False(ap.IsClosed);
        Assert.Empty(ap.Payments);
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => CreateSample(0m));
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => CreateSample(-100m));
    }

    [Fact]
    public void RecordPayment_ShouldReduceRemainingAmount()
    {
        var ap = CreateSample(2_000_000m);
        ap.ClearDomainEvents();

        ap.RecordPayment(1, 800_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);

        Assert.Equal(1_200_000m, ap.RemainingAmount);
        Assert.Single(ap.Payments);
        Assert.False(ap.IsClosed);
    }

    [Fact]
    public void RecordPayment_WhenFullyPaid_ShouldCloseAndRaiseApTransactionClosed()
    {
        var ap = CreateSample(2_000_000m);
        ap.ClearDomainEvents();

        ap.RecordPayment(1, 2_000_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);

        Assert.True(ap.IsClosed);
        Assert.Equal(0m, ap.RemainingAmount);

        var events = ap.DomainEvents;
        Assert.Equal(2, events.Count);
        Assert.IsType<ApTransactionPaymentRecorded>(events[0]);
        Assert.IsType<ApTransactionClosed>(events[1]);
    }

    [Fact]
    public void RecordPayment_WhenAlreadyClosed_ShouldThrowDomainException()
    {
        var ap = CreateSample(2_000_000m);
        ap.RecordPayment(1, 2_000_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);
        ap.ClearDomainEvents();

        Assert.Throws<DomainException>(() =>
            ap.RecordPayment(2, 100m, DateTime.UtcNow, "TRANSFER", "INV202506000002", "admin", DateTime.UtcNow));
    }

    [Fact]
    public void RecordPayment_WithZeroAmount_ShouldThrowDomainException()
    {
        var ap = CreateSample(2_000_000m);

        Assert.Throws<DomainException>(() =>
            ap.RecordPayment(1, 0m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow));
    }

    [Fact]
    public void Reconstitute_ShouldRestoreState()
    {
        var original = CreateSample(2_000_000m);
        original.RecordPayment(1, 800_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);

        var events = original.DomainEvents.ToList();
        var rebuilt = Domain.ApTransaction.ApTransaction.Reconstitute(events);

        Assert.Equal(original.Id, rebuilt.Id);
        Assert.Equal(1_200_000m, rebuilt.RemainingAmount);
        Assert.Single(rebuilt.Payments);
        Assert.False(rebuilt.IsClosed);
        Assert.Empty(rebuilt.DomainEvents);
    }

    [Fact]
    public void Reconstitute_WithFullPayment_ShouldShowClosed()
    {
        var original = CreateSample(2_000_000m);
        original.RecordPayment(1, 2_000_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);

        var events = original.DomainEvents.ToList();
        var rebuilt = Domain.ApTransaction.ApTransaction.Reconstitute(events);

        Assert.True(rebuilt.IsClosed);
        Assert.Equal(0m, rebuilt.RemainingAmount);
    }
}
