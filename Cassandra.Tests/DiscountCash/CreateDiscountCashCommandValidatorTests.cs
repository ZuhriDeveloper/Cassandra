using Cassandra.Application.Commands.DiscountCash.CreateDiscountCash;

namespace Cassandra.Tests.DiscountCash;

public class CreateDiscountCashCommandValidatorTests
{
    private readonly CreateDiscountCashCommandValidator _validator = new();

    private static CreateDiscountCashCommand Valid() => new(Guid.NewGuid(), 500_000m, 300_000m, "admin");

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(Valid(), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Empty_tipe_motor_id_fails()
    {
        var result = await _validator.ValidateAsync(Valid() with { TipeMotorId = Guid.Empty }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_direct_discount_fails()
    {
        var result = await _validator.ValidateAsync(Valid() with { DirectDiscount = -1m }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_channel_discount_fails()
    {
        var result = await _validator.ValidateAsync(Valid() with { ChannelDiscount = -1m }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Zero_discounts_pass()
    {
        var result = await _validator.ValidateAsync(Valid() with { DirectDiscount = 0m, ChannelDiscount = 0m }, TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }
}
