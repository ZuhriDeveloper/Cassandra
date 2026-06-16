using Cassandra.Application.Commands.DaftarHargaLeasing.CreateDaftarHargaLeasing;

namespace Cassandra.Tests.DaftarHargaLeasing;

public class CreateDaftarHargaLeasingCommandValidatorTests
{
    private readonly CreateDaftarHargaLeasingCommandValidator _validator = new();

    private static CreateDaftarHargaLeasingCommand Valid() =>
        new("DHL 2024", Guid.NewGuid(), Guid.NewGuid(), "admin");

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

    [Fact]
    public async Task Empty_global_leasing_id_fails()
    {
        var result = await _validator.ValidateAsync(Valid() with { GlobalLeasingId = Guid.Empty }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Empty_grup_tenor_id_fails()
    {
        var result = await _validator.ValidateAsync(Valid() with { GrupTenorId = Guid.Empty }, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
