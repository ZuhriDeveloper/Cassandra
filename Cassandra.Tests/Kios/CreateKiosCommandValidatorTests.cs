using Cassandra.Application.Commands.Kios.CreateKios;

namespace Cassandra.Tests.Kios;

public class CreateKiosCommandValidatorTests
{
    private readonly CreateKiosCommandValidator _validator = new();
    private static readonly Guid PicKaryawanId = Guid.NewGuid();

    private static CreateKiosCommand Valid() => new(
        "K001", "Kios Utama", "021-12345", null, "Jl. Merdeka 1", PicKaryawanId, 0m, "admin");

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(Valid(), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Kios Utama", "021-12345")]
    [InlineData("   ", "Kios Utama", "021-12345")]
    [InlineData("K001", "", "021-12345")]
    [InlineData("K001", "   ", "021-12345")]
    [InlineData("K001", "Kios Utama", "")]
    public async Task Invalid_required_fields_fail(string code, string name, string phone)
    {
        var cmd = Valid() with { Code = code, Name = name, Phone = phone };
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
    public async Task Empty_pic_fails()
    {
        var cmd = Valid() with { PicKaryawanId = Guid.Empty };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Code_exceeding_20_chars_fails()
    {
        var cmd = Valid() with { Code = new string('K', 21) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Fax_exceeding_20_chars_fails()
    {
        var cmd = Valid() with { Fax = new string('1', 21) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
