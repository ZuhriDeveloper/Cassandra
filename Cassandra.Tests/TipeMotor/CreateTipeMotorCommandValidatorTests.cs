using Cassandra.Application.Commands.TipeMotor.CreateTipeMotor;

namespace Cassandra.Tests.TipeMotor;

public class CreateTipeMotorCommandValidatorTests
{
    private readonly CreateTipeMotorCommandValidator _validator = new();
    private static readonly Guid GrupId = Guid.NewGuid();

    private static CreateTipeMotorCommand Valid() => new(
        "CB150", GrupId, "CB150R", "PC001", "WMS001", "AHM001",
        "NXXX", "CXXX", 50_000_000m, 1_000_000m, 900_000m, 500_000m, 450_000m, "admin");

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(Valid(), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_code_fails(string code)
    {
        var result = await _validator.ValidateAsync(Valid() with { Code = code }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Code_exceeding_20_chars_fails()
    {
        var cmd = Valid() with { Code = new string('X', 21) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_shortName_fails(string shortName)
    {
        var result = await _validator.ValidateAsync(Valid() with { ShortName = shortName }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ShortName_exceeding_100_chars_fails()
    {
        var cmd = Valid() with { ShortName = new string('X', 101) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Empty_grupTipeMotorId_fails()
    {
        var cmd = Valid() with { GrupTipeMotorId = Guid.Empty };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_nettPrice_fails()
    {
        var cmd = Valid() with { NettPrice = -1m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_orJakarta_fails()
    {
        var cmd = Valid() with { OrJakarta = -1m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_orTangerang_fails()
    {
        var cmd = Valid() with { OrTangerang = -1m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_bbnJakarta_fails()
    {
        var cmd = Valid() with { BbnJakarta = -1m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_bbnTangerang_fails()
    {
        var cmd = Valid() with { BbnTangerang = -1m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
