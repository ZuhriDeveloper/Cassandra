using Cassandra.Application.Commands.CabangLeasing.CreateCabangLeasing;

namespace Cassandra.Tests.CabangLeasing;

public class CreateCabangLeasingCommandValidatorTests
{
    private readonly CreateCabangLeasingCommandValidator _validator = new();

    private static CreateCabangLeasingCommand Valid() =>
        new("BCA-JKT", "BCA Jakarta", "021-5555", null, "John", Guid.NewGuid(), "admin");

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

    [Fact]
    public async Task Empty_global_leasing_id_fails()
    {
        var result = await _validator.ValidateAsync(Valid() with { GlobalLeasingId = Guid.Empty }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Phone_exceeding_30_chars_fails()
    {
        var cmd = Valid() with { Phone = new string('1', 31) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
