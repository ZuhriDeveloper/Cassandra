using Cassandra.Application.Commands.Dealers.RegisterDealer;

namespace Cassandra.Tests.Dealers;

public class RegisterDealerCommandValidatorTests
{
    private readonly RegisterDealerCommandValidator _validator = new();

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(
            new RegisterDealerCommand("Dealer Pusat", "D1", "superadmin@cassandra.local"),
            TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "D1")]
    [InlineData("Dealer", "")]
    public async Task Missing_required_fields_fail(string name, string code)
    {
        var result = await _validator.ValidateAsync(
            new RegisterDealerCommand(name, code, "superadmin@cassandra.local"),
            TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task Empty_registeredBy_fails()
    {
        var result = await _validator.ValidateAsync(
            new RegisterDealerCommand("Dealer Pusat", "D1", ""),
            TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDealerCommand.RegisteredBy));
    }
}
