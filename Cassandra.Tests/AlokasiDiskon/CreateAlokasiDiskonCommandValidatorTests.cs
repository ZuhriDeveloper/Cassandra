using Cassandra.Application.Commands.AlokasiDiskon.CreateAlokasiDiskon;

namespace Cassandra.Tests.AlokasiDiskon;

public class CreateAlokasiDiskonCommandValidatorTests
{
    private readonly CreateAlokasiDiskonCommandValidator _validator = new();

    private static CreateAlokasiDiskonCommand Valid() => new(Guid.NewGuid(), "GOLD", "admin");

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(Valid(), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Empty_karyawan_id_fails()
    {
        var result = await _validator.ValidateAsync(Valid() with { KaryawanId = Guid.Empty }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_discount_level_fails(string level)
    {
        var result = await _validator.ValidateAsync(Valid() with { DiscountLevel = level }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task DiscountLevel_exceeding_20_chars_fails()
    {
        var cmd = Valid() with { DiscountLevel = new string('X', 21) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
