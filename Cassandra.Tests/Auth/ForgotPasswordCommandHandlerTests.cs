using Cassandra.Application.Commands.Auth;
using Cassandra.Application.Contracts.Auth;
using Cassandra.Application.Contracts.Email;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cassandra.Tests.Auth;

/// <summary>
/// The forgot-password use case must (1) email a reset link only when the account exists,
/// (2) never reveal whether an email is registered, and (3) treat SMTP failure as non-fatal.
/// </summary>
public class ForgotPasswordCommandHandlerTests
{
    private readonly FakeUserAccountService _accounts = new();
    private readonly FakeEmailSender _email = new();

    private ForgotPasswordCommandHandler CreateHandler() =>
        new(_accounts, new FakeAccountLinkBuilder(), _email, NullLogger<ForgotPasswordCommandHandler>.Instance);

    [Fact]
    public async Task Known_email_sends_reset_email_with_link()
    {
        _accounts.ResetTokenToReturn =
            new PasswordResetTokenInfo("user-1", "budi@cassandra.local", "Budi", "tok-123");

        await CreateHandler().HandleAsync(
            new ForgotPasswordCommand("budi@cassandra.local"), TestContext.Current.CancellationToken);

        var sent = Assert.Single(_email.Sent);
        Assert.Equal("budi@cassandra.local", sent.ToEmail);
        Assert.Contains("tok-123", sent.HtmlBody);         // link carries the reset token
        Assert.Contains("reset-password", sent.HtmlBody);
    }

    [Fact]
    public async Task Unknown_email_sends_nothing_and_does_not_throw()
    {
        _accounts.ResetTokenToReturn = null; // no such user

        await CreateHandler().HandleAsync(
            new ForgotPasswordCommand("ghost@cassandra.local"), TestContext.Current.CancellationToken);

        Assert.Empty(_email.Sent);
        Assert.Equal("ghost@cassandra.local", _accounts.PasswordResetRequestedFor);
    }

    [Fact]
    public async Task Email_delivery_failure_is_swallowed()
    {
        _accounts.ResetTokenToReturn =
            new PasswordResetTokenInfo("user-1", "budi@cassandra.local", "Budi", "tok-123");
        _email.ThrowOnSend = true;

        var exception = await Record.ExceptionAsync(() =>
            CreateHandler().HandleAsync(new ForgotPasswordCommand("budi@cassandra.local"), TestContext.Current.CancellationToken));

        Assert.Null(exception);
    }

    private sealed class FakeUserAccountService : IUserAccountService
    {
        public PasswordResetTokenInfo? ResetTokenToReturn { get; set; }
        public AccountOperationResult ResultToReturn { get; set; } = AccountOperationResult.Ok();
        public string? PasswordResetRequestedFor { get; private set; }

        public Task<AccountOperationResult> ChangePasswordAsync(
            string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
            => Task.FromResult(ResultToReturn);

        public Task<PasswordResetTokenInfo?> CreatePasswordResetTokenAsync(
            string email, CancellationToken cancellationToken = default)
        {
            PasswordResetRequestedFor = email;
            return Task.FromResult(ResetTokenToReturn);
        }

        public Task<AccountOperationResult> ResetPasswordAsync(
            string email, string encodedToken, string newPassword, CancellationToken cancellationToken = default)
            => Task.FromResult(ResultToReturn);
    }

    private sealed class FakeAccountLinkBuilder : IAccountLinkBuilder
    {
        public string BuildPasswordResetLink(string email, string encodedToken) =>
            $"https://test/reset-password?email={email}&token={encodedToken}";
    }

    private sealed class FakeEmailSender : IEmailSender
    {
        public record SentEmail(string ToEmail, string? ToName, string Subject, string HtmlBody);

        public List<SentEmail> Sent { get; } = [];
        public bool ThrowOnSend { get; set; }

        public Task SendAsync(
            string toEmail, string? toName, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            if (ThrowOnSend)
                throw new InvalidOperationException("SMTP unavailable");

            Sent.Add(new SentEmail(toEmail, toName, subject, htmlBody));
            return Task.CompletedTask;
        }
    }
}
