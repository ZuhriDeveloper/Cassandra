using Cassandra.Application.Commands.Mediator.CreateMediator;

namespace Cassandra.Tests.Mediator;

public class CreateMediatorCommandValidatorTests
{
    private readonly CreateMediatorCommandValidator _validator = new();
    private static readonly Guid KaryawanId = Guid.NewGuid();

    private static CreateMediatorCommand Valid() =>
        new("Agen Utama", KaryawanId, "Jl. Agen No. 1", 0m, "admin");

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
        var cmd = Valid() with { Name = name };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Empty_karyawan_fails()
    {
        var cmd = Valid() with { KaryawanId = Guid.Empty };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_limit_fails()
    {
        var cmd = Valid() with { Limit = -1m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Name_exceeding_200_chars_fails()
    {
        var cmd = Valid() with { Name = new string('A', 201) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
