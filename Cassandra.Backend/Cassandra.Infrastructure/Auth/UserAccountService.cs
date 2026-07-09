using Cassandra.Application.Contracts.Auth;
using Cassandra.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Cassandra.Infrastructure.Auth;

/// <summary>
/// Implements the self-service account operations over ASP.NET Identity's
/// <see cref="UserManager{TUser}"/> token providers (registered via AddDefaultTokenProviders).
/// </summary>
public class UserAccountService(UserManager<ApplicationUser> userManager) : IUserAccountService
{
    public async Task<AccountOperationResult> ChangePasswordAsync(
        string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return AccountOperationResult.Fail("User not found.");

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return ToResult(result);
    }

    public async Task<PasswordResetTokenInfo?> CreatePasswordResetTokenAsync(
        string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return new PasswordResetTokenInfo(user.Id, user.Email!, user.FullName, IdentityTokenCodec.Encode(token));
    }

    public async Task<AccountOperationResult> ResetPasswordAsync(
        string email, string encodedToken, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return AccountOperationResult.Fail("Invalid or expired password reset link.");

        if (!TryDecode(encodedToken, out var token))
            return AccountOperationResult.Fail("Invalid or expired password reset link.");

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);

        // Anti-enumeration: an unknown email, a bad token, and an expired token must all yield the
        // same generic message so an attacker without a valid token cannot tell registered emails
        // apart. Identity validates the token before the password, so an InvalidToken failure never
        // co-occurs with password-policy errors — those only reach a caller who already holds a
        // valid token (the legitimate user), so they stay specific to give useful feedback.
        if (!result.Succeeded && result.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.InvalidToken)))
            return AccountOperationResult.Fail("Invalid or expired password reset link.");

        return ToResult(result);
    }

    private static bool TryDecode(string encodedToken, out string token)
    {
        try
        {
            token = IdentityTokenCodec.Decode(encodedToken);
            return true;
        }
        catch
        {
            token = string.Empty;
            return false;
        }
    }

    private static AccountOperationResult ToResult(IdentityResult result) =>
        result.Succeeded
            ? AccountOperationResult.Ok()
            : AccountOperationResult.Fail(result.Errors.Select(e => e.Description));
}
