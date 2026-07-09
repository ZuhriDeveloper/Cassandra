using System.Net.Http.Json;

namespace Cassandra.WebUi.Services;

/// <summary>
/// Authenticated account self-service operations (carries the JWT via BearerTokenHandler).
/// Used by the signed-in "change password" page.
/// </summary>
public class AccountApiClient(HttpClient http)
{
    public async Task<AccountResult> ChangePasswordAsync(
        string currentPassword, string newPassword, CancellationToken ct = default)
    {
        try
        {
            var response = await http.PostAsJsonAsync("api/auth/change-password",
                new { currentPassword, newPassword }, ct);
            return await AccountResult.FromResponseAsync(response);
        }
        catch
        {
            return AccountResult.Failure(["Tidak dapat terhubung ke server. Silakan coba lagi."]);
        }
    }
}
