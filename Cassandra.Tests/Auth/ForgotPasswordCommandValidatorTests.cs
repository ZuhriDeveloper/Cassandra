using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Validators.Auth;

namespace Cassandra.Tests.Auth;

public class ForgotPasswordCommandValidatorTests
{
    private readonly ForgotPasswordCommandValidator _validator = new();

    [Fact]
    public async Task Valid_email_passes()
    {
        var result = await _validator.ValidateAsync(
            new ForgotPasswordCommand("budi@cassandra.local"), TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task Invalid_email_fails(string email)
    {
        var result = await _validator.ValidateAsync(
            new ForgotPasswordCommand(email), TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ForgotPasswordCommand.Email));
    }
}
