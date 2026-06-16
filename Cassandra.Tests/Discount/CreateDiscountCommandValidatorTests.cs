using Cassandra.Application.Commands.Discount.CreateDiscount;

namespace Cassandra.Tests.Discount;

public class CreateDiscountCommandValidatorTests
{
    private readonly CreateDiscountCommandValidator _validator = new();

    private static CreateDiscountCommand Valid() => new(Guid.NewGuid(), "GOLD", "admin");

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(Valid(), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Empty_daftar_harga_leasing_id_fails()
    {
        var result = await _validator.ValidateAsync(Valid() with { DaftarHargaLeasingId = Guid.Empty }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_level_fails(string level)
    {
        var result = await _validator.ValidateAsync(Valid() with { Level = level }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Level_exceeding_20_chars_fails()
    {
        var cmd = Valid() with { Level = new string('X', 21) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
