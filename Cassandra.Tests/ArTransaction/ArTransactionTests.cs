using Cassandra.Domain.ArTransaction;
using Cassandra.Domain.ArTransaction.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.ArTransaction;

public class ArTransactionTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly ArTransactionId Id = ArTransactionId.New();

    private static Domain.ArTransaction.ArTransaction CreateSample(decimal totalAmount = 1_000_000m) =>
        Domain.ArTransaction.ArTransaction.Create(
            Id,
            Domain.ArTransaction.ArTransaction.PENJUALAN,
            Guid.NewGuid(),
            "NP-001",
            totalAmount,
            "admin",
            DealerId);

    [Fact]
    public void Create_ShouldRaiseArTransactionCreated()
    {
        var ar = CreateSample();

        var evt = Assert.IsType<ArTransactionCreated>(Assert.Single(ar.DomainEvents));
        Assert.Equal(Domain.ArTransaction.ArTransaction.PENJUALAN, evt.TransactionType);
        Assert.Equal(1_000_000m, evt.TotalAmount);
        Assert.Equal(DealerId, evt.DealerId);
    }

    [Fact]
    public void Create_SetsInitialState()
    {
        var ar = CreateSample(500_000m);

        Assert.Equal(500_000m, ar.TotalAmount);
        Assert.Equal(500_000m, ar.RemainingAmount);
        Assert.False(ar.IsClosed);
        Assert.Empty(ar.Payments);
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
        var ar = CreateSample(1_000_000m);
        ar.ClearDomainEvents();

        ar.RecordPayment(1, 400_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);

        Assert.Equal(600_000m, ar.RemainingAmount);
        Assert.Single(ar.Payments);
        Assert.False(ar.IsClosed);
    }

    [Fact]
    public void RecordPayment_WhenFullyPaid_ShouldCloseAndRaiseArTransactionClosed()
    {
        var ar = CreateSample(1_000_000m);
        ar.ClearDomainEvents();

        ar.RecordPayment(1, 1_000_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);

        Assert.True(ar.IsClosed);
        Assert.Equal(0m, ar.RemainingAmount);

        var events = ar.DomainEvents;
        Assert.Equal(2, events.Count);
        Assert.IsType<ArTransactionPaymentRecorded>(events[0]);
        Assert.IsType<ArTransactionClosed>(events[1]);
    }

    [Fact]
    public void RecordPayment_WhenAlreadyClosed_ShouldThrowDomainException()
    {
        var ar = CreateSample(1_000_000m);
        ar.RecordPayment(1, 1_000_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);
        ar.ClearDomainEvents();

        Assert.Throws<DomainException>(() =>
            ar.RecordPayment(2, 100m, DateTime.UtcNow, "TRANSFER", "INV202506000002", "admin", DateTime.UtcNow));
    }

    [Fact]
    public void RecordPayment_WithZeroAmount_ShouldThrowDomainException()
    {
        var ar = CreateSample(1_000_000m);

        Assert.Throws<DomainException>(() =>
            ar.RecordPayment(1, 0m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow));
    }

    [Fact]
    public void Reconstitute_ShouldRestoreState()
    {
        var original = CreateSample(1_000_000m);
        original.RecordPayment(1, 400_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);

        var events = original.DomainEvents.ToList();
        var rebuilt = Domain.ArTransaction.ArTransaction.Reconstitute(events);

        Assert.Equal(original.Id, rebuilt.Id);
        Assert.Equal(600_000m, rebuilt.RemainingAmount);
        Assert.Single(rebuilt.Payments);
        Assert.False(rebuilt.IsClosed);
        Assert.Empty(rebuilt.DomainEvents);
    }

    [Fact]
    public void Reconstitute_WithFullPayment_ShouldShowClosed()
    {
        var original = CreateSample(1_000_000m);
        original.RecordPayment(1, 1_000_000m, DateTime.UtcNow, "TRANSFER", "INV202506000001", "admin", DateTime.UtcNow);

        var events = original.DomainEvents.ToList();
        var rebuilt = Domain.ArTransaction.ArTransaction.Reconstitute(events);

        Assert.True(rebuilt.IsClosed);
        Assert.Equal(0m, rebuilt.RemainingAmount);
    }
}
