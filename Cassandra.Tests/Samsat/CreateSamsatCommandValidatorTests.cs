using Cassandra.Application.Commands.Samsat.CreateSamsat;

namespace Cassandra.Tests.Samsat;

public class CreateSamsatCommandValidatorTests
{
    private readonly CreateSamsatCommandValidator _validator = new();

    private static CreateSamsatCommand Valid() => new("Samsat Jakarta Barat", "admin");

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(Valid(), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
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
    public async Task Name_exceeding_200_chars_fails()
    {
        var cmd = Valid() with { Name = new string('X', 201) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
