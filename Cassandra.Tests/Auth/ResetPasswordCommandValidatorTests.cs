using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Validators.Auth;

namespace Cassandra.Tests.Auth;

public class ResetPasswordCommandValidatorTests
{
    private readonly ResetPasswordCommandValidator _validator = new();

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(
            new ResetPasswordCommand("budi@cassandra.local", "tok-123", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task Invalid_email_fails(string email)
    {
        var result = await _validator.ValidateAsync(
            new ResetPasswordCommand(email, "tok-123", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ResetPasswordCommand.Email));
    }

    [Fact]
    public async Task Token_is_required()
    {
        var result = await _validator.ValidateAsync(
            new ResetPasswordCommand("budi@cassandra.local", "", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ResetPasswordCommand.Token));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short1A")]
    [InlineData("nouppercase1")]
    [InlineData("NoDigitHere")]
    public async Task New_password_must_satisfy_password_rules(string newPassword)
    {
        var result = await _validator.ValidateAsync(
            new ResetPasswordCommand("budi@cassandra.local", "tok-123", newPassword), TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ResetPasswordCommand.NewPassword));
    }
}
