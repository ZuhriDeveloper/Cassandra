using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Contracts.Auth;

namespace Cassandra.Tests.Auth;

public class LoginCommandHandlerTests
{
    private const string KnownEmail = "admin@cassandra.local";
    private const string KnownId = "user-1";
    private const string CorrectPassword = "Admin@1234";

    [Fact]
    public async Task Returns_token_and_roles_on_valid_credentials()
    {
        var repo = new FakeUserAuthRepository
        {
            User = new UserAuthInfo(KnownId, KnownEmail, "Admin Cassandra"),
            Password = CorrectPassword,
            Roles = ["Admin"],
        };
        var handler = new LoginCommandHandler(repo, new FakeTokenService());

        var result = await handler.HandleAsync(new LoginCommand(KnownEmail, CorrectPassword), TestContext.Current.CancellationToken);

        Assert.True(result.Succeeded);
        Assert.Equal("token-for-" + KnownId, result.Token);
        Assert.Equal(KnownEmail, result.Email);
        Assert.Equal("Admin Cassandra", result.FullName);
        Assert.Equal(["Admin"], result.Roles);
        Assert.True(repo.ResetCalled);
        Assert.False(repo.RecordFailedCalled);
    }

    [Fact]
    public async Task Records_failure_and_fails_on_wrong_password()
    {
        var repo = new FakeUserAuthRepository
        {
            User = new UserAuthInfo(KnownId, KnownEmail, "Admin Cassandra"),
            Password = CorrectPassword,
            Roles = ["Admin"],
        };
        var handler = new LoginCommandHandler(repo, new FakeTokenService());

        var result = await handler.HandleAsync(new LoginCommand(KnownEmail, "wrong-password"), TestContext.Current.CancellationToken);

        Assert.False(result.Succeeded);
        Assert.Equal("Invalid email or password.", result.ErrorMessage);
        Assert.True(repo.RecordFailedCalled);
        Assert.False(repo.ResetCalled);
    }

    [Fact]
    public async Task Fails_when_account_locked_out()
    {
        var repo = new FakeUserAuthRepository
        {
            User = new UserAuthInfo(KnownId, KnownEmail, "Admin Cassandra"),
            Password = CorrectPassword,
            LockedOut = true,
        };
        var handler = new LoginCommandHandler(repo, new FakeTokenService());

        var result = await handler.HandleAsync(new LoginCommand(KnownEmail, CorrectPassword), TestContext.Current.CancellationToken);

        Assert.False(result.Succeeded);
        Assert.Contains("locked", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        Assert.False(repo.RecordFailedCalled);
    }

    [Fact]
    public async Task Fails_when_email_unknown()
    {
        var repo = new FakeUserAuthRepository { User = null };
        var handler = new LoginCommandHandler(repo, new FakeTokenService());

        var result = await handler.HandleAsync(new LoginCommand("nobody@cassandra.local", "whatever1"), TestContext.Current.CancellationToken);

        Assert.False(result.Succeeded);
        Assert.Equal("Invalid email or password.", result.ErrorMessage);
    }

    private sealed class FakeUserAuthRepository : IUserAuthRepository
    {
        public UserAuthInfo? User { get; init; }
        public string? Password { get; init; }
        public IReadOnlyList<string> Roles { get; init; } = [];
        public bool LockedOut { get; init; }
        public bool RecordFailedCalled { get; private set; }
        public bool ResetCalled { get; private set; }

        public Task<UserAuthInfo?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(User is not null && User.Email == email ? User : null);

        public Task<bool> CheckPasswordAsync(string userId, string password)
            => Task.FromResult(password == Password);

        public Task<bool> IsLockedOutAsync(string userId) => Task.FromResult(LockedOut);

        public Task RecordFailedAccessAsync(string userId)
        {
            RecordFailedCalled = true;
            return Task.CompletedTask;
        }

        public Task ResetAccessFailedCountAsync(string userId)
        {
            ResetCalled = true;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<string>> GetRolesAsync(string userId, CancellationToken cancellationToken = default)
            => Task.FromResult(Roles);
    }

    private sealed class FakeTokenService : ITokenService
    {
        public string GenerateToken(UserAuthInfo user, IReadOnlyList<string> roles) => "token-for-" + user.Id;
    }
}
