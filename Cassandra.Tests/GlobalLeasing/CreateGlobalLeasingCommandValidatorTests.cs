using Cassandra.Application.Commands.GlobalLeasing.CreateGlobalLeasing;

namespace Cassandra.Tests.GlobalLeasing;

public class CreateGlobalLeasingCommandValidatorTests
{
    private readonly CreateGlobalLeasingCommandValidator _validator = new();

    private static CreateGlobalLeasingCommand Valid() =>
        new("BCA", "BCA Finance", "021-5555", null, "John", "Jakarta", "admin");

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
    public async Task Name_exceeding_200_chars_fails()
    {
        var cmd = Valid() with { Name = new string('X', 201) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_phone_fails(string phone)
    {
        var result = await _validator.ValidateAsync(Valid() with { Phone = phone }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_contact_fails(string contact)
    {
        var result = await _validator.ValidateAsync(Valid() with { Contact = contact }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
