using Cassandra.Application.Commands.Tenor.CreateTenor;

namespace Cassandra.Tests.Tenor;

public class CreateTenorCommandValidatorTests
{
    private readonly CreateTenorCommandValidator _validator = new();

    private static CreateTenorCommand Valid() => new("T12", "12 Bulan", 12, Guid.NewGuid(), "admin");

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

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Months_zero_or_negative_fails(int months)
    {
        var result = await _validator.ValidateAsync(Valid() with { Months = months }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Empty_grup_tenor_id_fails()
    {
        var result = await _validator.ValidateAsync(Valid() with { GrupTenorId = Guid.Empty }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
