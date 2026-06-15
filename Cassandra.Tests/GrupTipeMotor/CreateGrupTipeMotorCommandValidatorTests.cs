using Cassandra.Application.Commands.GrupTipeMotor.CreateGrupTipeMotor;

namespace Cassandra.Tests.GrupTipeMotor;

public class CreateGrupTipeMotorCommandValidatorTests
{
    private readonly CreateGrupTipeMotorCommandValidator _validator = new();

    private static CreateGrupTipeMotorCommand Valid() => new("SPORT", "admin");

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
}
