using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Validators.Auth;

namespace Cassandra.Tests.Auth;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(new LoginCommand("admin@cassandra.local", "Admin@1234"), TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task Invalid_email_fails(string email)
    {
        var result = await _validator.ValidateAsync(new LoginCommand(email, "Admin@1234"), TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginCommand.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    public async Task Invalid_password_fails(string password)
    {
        var result = await _validator.ValidateAsync(new LoginCommand("admin@cassandra.local", password), TestContext.Current.CancellationToken);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginCommand.Password));
    }
}
