using Cassandra.Application.Commands.So.CreateSo;
using Cassandra.Domain.So;

namespace Cassandra.Tests.So;

public class CreateSoCommandValidatorTests
{
    private readonly CreateSoCommandValidator _validator = new();

    private static CreateSoCommand ValidCommand() =>
        new(
            "SO-001",
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
            SoPaymentType.CASH,
            Guid.NewGuid(),
            0m, 0m, 0m,
            [new CreateSoItemRequest(Guid.NewGuid(), Guid.NewGuid(), 1, 25_000_000m)],
            "admin");

    [Fact]
    public async Task Valid_Command_PassesValidation()
    {
        var result = await _validator.ValidateAsync(ValidCommand(), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Fails_WhenSoNumberEmpty(string soNumber)
    {
        var cmd = ValidCommand() with { SoNumber = soNumber };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateSoCommand.SoNumber));
    }

    [Theory]
    [InlineData("TUNAI")]
    [InlineData("")]
    public async Task Fails_WhenInvalidPaymentType(string paymentType)
    {
        var cmd = ValidCommand() with { PaymentType = paymentType };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Fails_WhenItemsEmpty()
    {
        var cmd = ValidCommand() with { Items = [] };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateSoCommand.Items));
    }

    [Fact]
    public async Task Fails_WhenItemQtyIsZero()
    {
        var cmd = ValidCommand() with
        {
            Items = [new CreateSoItemRequest(Guid.NewGuid(), Guid.NewGuid(), 0, 1000m)]
        };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Fails_WhenItemNettPriceNegative()
    {
        var cmd = ValidCommand() with
        {
            Items = [new CreateSoItemRequest(Guid.NewGuid(), Guid.NewGuid(), 1, -1m)]
        };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
