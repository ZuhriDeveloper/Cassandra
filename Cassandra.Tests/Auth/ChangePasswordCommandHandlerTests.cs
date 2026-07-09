using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Contracts.Auth;

namespace Cassandra.Tests.Auth;

public class ChangePasswordCommandHandlerTests
{
    private readonly FakeUserAccountService _accounts = new();

    [Fact]
    public async Task Delegates_to_account_service_with_user_id()
    {
        var handler = new ChangePasswordCommandHandler(_accounts);

        var result = await handler.HandleAsync(
            new ChangePasswordCommand("user-7", "OldPass1", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.Equal("user-7", _accounts.ChangePasswordUserId);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Propagates_failure()
    {
        _accounts.ResultToReturn = AccountOperationResult.Fail("Incorrect password.");
        var handler = new ChangePasswordCommandHandler(_accounts);

        var result = await handler.HandleAsync(
            new ChangePasswordCommand("user-7", "wrong", "NewPass1"), TestContext.Current.CancellationToken);

        Assert.False(result.Succeeded);
        Assert.Contains("Incorrect password.", result.Errors);
    }

    private sealed class FakeUserAccountService : IUserAccountService
    {
        public AccountOperationResult ResultToReturn { get; set; } = AccountOperationResult.Ok();
        public string? ChangePasswordUserId { get; private set; }

        public Task<AccountOperationResult> ChangePasswordAsync(
            string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            ChangePasswordUserId = userId;
            return Task.FromResult(ResultToReturn);
        }

        public Task<PasswordResetTokenInfo?> CreatePasswordResetTokenAsync(
            string email, CancellationToken cancellationToken = default)
            => Task.FromResult<PasswordResetTokenInfo?>(null);

        public Task<AccountOperationResult> ResetPasswordAsync(
            string email, string encodedToken, string newPassword, CancellationToken cancellationToken = default)
            => Task.FromResult(ResultToReturn);
    }
}
