using Cassandra.Application.Commands.PelanggaranWilayah.CreatePelanggaranWilayah;

namespace Cassandra.Tests.PelanggaranWilayah;

public class CreatePelanggaranWilayahCommandValidatorTests
{
    private readonly CreatePelanggaranWilayahCommandValidator _validator = new();

    private static CreatePelanggaranWilayahCommand Valid() => new("021", 500000m, "admin");

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(Valid(), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_area_code_fails(string areaCode)
    {
        var result = await _validator.ValidateAsync(Valid() with { AreaCode = areaCode }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task AreaCode_exceeding_50_chars_fails()
    {
        var cmd = Valid() with { AreaCode = new string('X', 51) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_extra_fee_fails()
    {
        var cmd = Valid() with { ExtraFee = -1m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Zero_extra_fee_passes()
    {
        var cmd = Valid() with { ExtraFee = 0m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }
}
