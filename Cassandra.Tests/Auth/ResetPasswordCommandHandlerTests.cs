using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Contracts.Auth;

namespace Cassandra.Tests.Auth;

public class ResetPasswordCommandHandlerTests
{
    private readonly FakeUserAccountService _accounts = new();

    [Fact]
    public async Task Forwards_arguments_to_account_service()
    {
        var handler = new ResetPasswordCommandHandler(_accounts);

        await handler.HandleAsync(
            new ResetPasswordCommand("budi@cassandra.local", "tok-123", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.Equal(("budi@cassandra.local", "tok-123", "NewPass1"), _accounts.ResetPasswordCall);
    }

    [Fact]
    public async Task Propagates_failure_from_account_service()
    {
        _accounts.ResultToReturn = AccountOperationResult.Fail("Invalid or expired password reset link.");
        var handler = new ResetPasswordCommandHandler(_accounts);

        var result = await handler.HandleAsync(
            new ResetPasswordCommand("budi@cassandra.local", "bad", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.False(result.Succeeded);
        Assert.Contains("Invalid or expired password reset link.", result.Errors);
    }

    [Fact]
    public async Task Propagates_success_from_account_service()
    {
        _accounts.ResultToReturn = AccountOperationResult.Ok();
        var handler = new ResetPasswordCommandHandler(_accounts);

        var result = await handler.HandleAsync(
            new ResetPasswordCommand("budi@cassandra.local", "tok-123", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.True(result.Succeeded);
    }

    private sealed class FakeUserAccountService : IUserAccountService
    {
        public AccountOperationResult ResultToReturn { get; set; } = AccountOperationResult.Ok();
        public (string Email, string Token, string NewPassword)? ResetPasswordCall { get; private set; }

        public Task<AccountOperationResult> ChangePasswordAsync(
            string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
            => Task.FromResult(ResultToReturn);

        public Task<PasswordResetTokenInfo?> CreatePasswordResetTokenAsync(
            string email, CancellationToken cancellationToken = default)
            => Task.FromResult<PasswordResetTokenInfo?>(null);

        public Task<AccountOperationResult> ResetPasswordAsync(
            string email, string encodedToken, string newPassword, CancellationToken cancellationToken = default)
        {
            ResetPasswordCall = (email, encodedToken, newPassword);
            return Task.FromResult(ResultToReturn);
        }
    }
}
