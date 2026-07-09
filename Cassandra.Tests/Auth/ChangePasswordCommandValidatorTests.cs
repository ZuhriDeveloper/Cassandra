using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Validators.Auth;

namespace Cassandra.Tests.Auth;

public class ChangePasswordCommandValidatorTests
{
    private readonly ChangePasswordCommandValidator _validator = new();

    [Fact]
    public async Task Valid_command_passes()
    {
        var result = await _validator.ValidateAsync(
            new ChangePasswordCommand("user-1", "OldPass1", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Current_password_is_required()
    {
        var result = await _validator.ValidateAsync(
            new ChangePasswordCommand("user-1", "", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.CurrentPassword));
    }

    [Theory]
    [InlineData("")]
    [InlineData("short1A")]
    [InlineData("nouppercase1")]
    [InlineData("NoDigitHere")]
    public async Task New_password_must_satisfy_password_rules(string newPassword)
    {
        var result = await _validator.ValidateAsync(
            new ChangePasswordCommand("user-1", "OldPass1", newPassword), TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ChangePasswordCommand.NewPassword));
    }

    [Fact]
    public async Task New_password_must_differ_from_current_password()
    {
        var result = await _validator.ValidateAsync(
            new ChangePasswordCommand("user-1", "SamePass1", "SamePass1"), TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(ChangePasswordCommand.NewPassword) &&
            e.ErrorMessage.Contains("different", StringComparison.OrdinalIgnoreCase));
    }
}
