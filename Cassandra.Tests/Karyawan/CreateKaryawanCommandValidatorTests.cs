using Cassandra.Application.Commands.Karyawan.CreateKaryawan;
using Cassandra.Domain.Karyawan;

namespace Cassandra.Tests.Karyawan;

public class CreateKaryawanCommandValidatorTests
{
    private readonly CreateKaryawanCommandValidator _validator = new();
    private static readonly Guid JabatanId = Guid.NewGuid();

    private static CreateKaryawanCommand Valid() => new(
        "Budi Santoso", "budi@cassandra.local", "3201234567890001",
        Gender.Male, new DateOnly(2024, 1, 15),
        "08123456789", null, "Jl. Merdeka No. 1",
        0m, JabatanId, "admin");

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(Valid(), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "budi@x.com", "KTP")]
    [InlineData("   ", "budi@x.com", "KTP")]
    [InlineData("Budi", "", "KTP")]
    [InlineData("Budi", "not-an-email", "KTP")]
    [InlineData("Budi", "budi@x.com", "")]
    public async Task Invalid_fields_fail(string name, string email, string ktp)
    {
        var cmd = Valid() with { Name = name, Email = email, KtpNumber = ktp };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Negative_sales_limit_fails()
    {
        var cmd = Valid() with { SalesLimit = -1m };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Empty_jabatan_id_fails()
    {
        var cmd = Valid() with { JabatanId = Guid.Empty };
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

    [Fact]
    public async Task Phone_alt_exceeding_20_chars_fails()
    {
        var cmd = Valid() with { PhoneAlt = new string('1', 21) };
        var result = await _validator.ValidateAsync(cmd, TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }
}
