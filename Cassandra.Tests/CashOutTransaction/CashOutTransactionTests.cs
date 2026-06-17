using Cassandra.Domain.CashOutTransaction;
using Cassandra.Domain.CashOutTransaction.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Tests.CashOutTransaction;

public class CashOutTransactionTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly CashOutTransactionId Id = CashOutTransactionId.New();
    private static readonly Guid SoId = Guid.NewGuid();

    private static Domain.CashOutTransaction.CashOutTransaction CreateSample(decimal amount = 5_000_000m) =>
        Domain.CashOutTransaction.CashOutTransaction.Create(
            Id,
            Domain.CashOutTransaction.CashOutTransaction.FSO_CASH,
            SoId,
            null,
            amount,
            0m,
            0,
            DateTime.UtcNow,
            "TRANSFER",
            "INV202506000001",
            "admin",
            DealerId);

    [Fact]
    public void Create_ShouldRaiseCashOutTransactionCreated()
    {
        var cashOut = CreateSample();

        var evt = Assert.IsType<CashOutTransactionCreated>(Assert.Single(cashOut.DomainEvents));
        Assert.Equal(Domain.CashOutTransaction.CashOutTransaction.FSO_CASH, evt.TransactionType);
        Assert.Equal(5_000_000m, evt.Amount);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(SoId, evt.SoId);
        Assert.Null(evt.SoReturId);
    }

    [Fact]
    public void Create_ShouldBeImmediatelyClosed()
    {
        var cashOut = CreateSample();

        Assert.True(cashOut.IsClosed);
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
    public void Create_SetsCorrectFields()
    {
        var cashOut = CreateSample(3_000_000m);

        Assert.Equal(Domain.CashOutTransaction.CashOutTransaction.FSO_CASH, cashOut.TransactionType);
        Assert.Equal(3_000_000m, cashOut.Amount);
        Assert.Equal(0m, cashOut.DfAmount);
        Assert.Equal(0, cashOut.TotalHariDf);
        Assert.Equal("TRANSFER", cashOut.PaymentMethod);
        Assert.Equal("INV202506000001", cashOut.FInvoiceId);
        Assert.Equal(SoId, cashOut.SoId);
        Assert.Null(cashOut.SoReturId);
    }

    [Fact]
    public void Reconstitute_ShouldRestoreState()
    {
        var original = CreateSample(5_000_000m);

        var events = original.DomainEvents.ToList();
        var rebuilt = Domain.CashOutTransaction.CashOutTransaction.Reconstitute(events);

        Assert.Equal(original.Id, rebuilt.Id);
        Assert.Equal(5_000_000m, rebuilt.Amount);
        Assert.True(rebuilt.IsClosed);
        Assert.Equal(DealerId, rebuilt.DealerId);
        Assert.Empty(rebuilt.DomainEvents);
    }
}
