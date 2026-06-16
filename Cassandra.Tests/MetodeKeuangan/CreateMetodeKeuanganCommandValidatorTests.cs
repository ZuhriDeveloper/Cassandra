using Cassandra.Application.Commands.MetodeKeuangan.CreateMetodeKeuangan;

namespace Cassandra.Tests.MetodeKeuangan;

public class CreateMetodeKeuanganCommandValidatorTests
{
    private readonly CreateMetodeKeuanganCommandValidator _validator = new();

    private static CreateMetodeKeuanganCommand Valid() => new("KAS", "Kas", "admin");

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
    public async Task Name_exceeding_100_chars_fails()
    {
        var cmd = Valid() with { Name = new string('X', 101) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
