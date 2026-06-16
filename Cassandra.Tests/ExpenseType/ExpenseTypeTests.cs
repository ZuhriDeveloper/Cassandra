using Cassandra.Domain.Common;
using Cassandra.Domain.ExpenseType;
using Cassandra.Domain.ExpenseType.Events;

namespace Cassandra.Tests.ExpenseType;

public class ExpenseTypeTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.ExpenseType.ExpenseType MakeExpenseType(
        string code = "BP",
        string name = "Biaya Perawatan")
        => Domain.ExpenseType.ExpenseType.Create(code, name, "admin", DealerId);

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var et = MakeExpenseType();

        Assert.Single(et.DomainEvents);
        var evt = Assert.IsType<ExpenseTypeCreated>(et.DomainEvents[0]);

        Assert.Equal("BP", evt.Code);
        Assert.Equal("Biaya Perawatan", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal("BP", et.Code);
        Assert.Equal("Biaya Perawatan", et.Name);
        Assert.True(et.IsActive);
    }

    [Fact]
    public void Create_NormalisesCodeToUppercase()
    {
        var et = MakeExpenseType(code: "bp");
        Assert.Equal("BP", et.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_CodeEmpty(string code)
    {
        Assert.Throws<DomainException>(() => MakeExpenseType(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_NameEmpty(string name)
    {
        Assert.Throws<DomainException>(() => MakeExpenseType(name: name));
    }

    [Fact]
    public void Update_RaisesEvent_WhenNameChanges()
    {
        var et = MakeExpenseType();
        et.ClearDomainEvents();

        et.Update("Biaya Operasional", "admin");

        var evt = Assert.IsType<ExpenseTypeUpdated>(Assert.Single(et.DomainEvents));
        Assert.Equal("Biaya Operasional", evt.Name);
        Assert.Equal("Biaya Operasional", et.Name);
    }

    [Fact]
    public void Update_IsNoop_WhenNameSame()
    {
        var et = MakeExpenseType();
        et.ClearDomainEvents();

        et.Update("Biaya Perawatan", "admin");

        Assert.Empty(et.DomainEvents);
    }

    [Fact]
    public void Deactivate_SetsIsActive_False()
    {
        var et = MakeExpenseType();
        et.ClearDomainEvents();

        et.Deactivate("admin");

        Assert.False(et.IsActive);
        Assert.IsType<ExpenseTypeDeactivated>(Assert.Single(et.DomainEvents));
    }

    [Fact]
    public void Activate_SetsIsActive_True()
    {
        var et = MakeExpenseType();
        et.Deactivate("admin");
        et.ClearDomainEvents();

        et.Activate("admin");

        Assert.True(et.IsActive);
        Assert.IsType<ExpenseTypeActivated>(Assert.Single(et.DomainEvents));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var et = MakeExpenseType();
        et.Update("Biaya Baru", "admin");
        et.Deactivate("admin");

        var replayed = Domain.ExpenseType.ExpenseType.Reconstitute(et.DomainEvents);

        Assert.Equal(et.Id, replayed.Id);
        Assert.Equal(et.Code, replayed.Code);
        Assert.Equal(et.Name, replayed.Name);
        Assert.Equal(et.IsActive, replayed.IsActive);
        Assert.Equal(et.DealerId, replayed.DealerId);
    }
}
