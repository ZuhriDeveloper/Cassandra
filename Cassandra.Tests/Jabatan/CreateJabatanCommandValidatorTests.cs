using Cassandra.Application.Commands.Jabatan.CreateJabatan;

namespace Cassandra.Tests.Jabatan;

public class CreateJabatanCommandValidatorTests
{
    private readonly CreateJabatanCommandValidator _validator = new();

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(
            new CreateJabatanCommand("Kepala Mekanik", "Deskripsi jabatan", "admin"),
            TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "desc", "admin")]
    [InlineData("   ", "desc", "admin")]
    [InlineData("Name", "desc", "")]
    public async Task Invalid_command_fails(string name, string description, string createdBy)
    {
        var result = await _validator.ValidateAsync(
            new CreateJabatanCommand(name, description, createdBy),
            TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Name_exceeding_100_chars_fails()
    {
        var longName = new string('A', 101);

        var result = await _validator.ValidateAsync(
            new CreateJabatanCommand(longName, "desc", "admin"),
            TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Description_exceeding_500_chars_fails()
    {
        var longDesc = new string('A', 501);

        var result = await _validator.ValidateAsync(
            new CreateJabatanCommand("Name", longDesc, "admin"),
            TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
    }
}
