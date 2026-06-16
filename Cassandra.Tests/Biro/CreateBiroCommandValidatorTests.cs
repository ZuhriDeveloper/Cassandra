using Cassandra.Application.Commands.Biro.CreateBiro;

namespace Cassandra.Tests.Biro;

public class CreateBiroCommandValidatorTests
{
    private readonly CreateBiroCommandValidator _validator = new();

    private static CreateBiroCommand Valid() =>
        new("BR001", "Biro Jasa Maju", null, null, null, null, 2.5m, "admin");

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
    public async Task Empty_name_fails(string name)
    {
        var result = await _validator.ValidateAsync(Valid() with { Name = name }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_pph_rate_fails()
    {
        var cmd = Valid() with { PphRate = -0.1m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Zero_pph_rate_passes()
    {
        var cmd = Valid() with { PphRate = 0m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }
}
