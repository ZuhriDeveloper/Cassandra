using System.Net.Http.Json;

namespace Cassandra.WebUi.Services;

/// <summary>
/// Client for a dealer Admin to provision their own staff (Sales / Cashier). The dealer is
/// taken from the caller's token server-side, so it is never sent from here.
/// </summary>
public class DealerUsersApiClient(HttpClient http)
{
    public async Task<(string? UserId, List<string>? Errors)> RegisterStaffAsync(
        string email, string fullName, string password, string role,
        CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/users",
            new { email, fullName, password, role }, ct);

        if (!response.IsSuccessStatusCode)
            return (null, await PlatformApiClient.ReadErrorsAsync(response));

        var result = await response.Content
            .ReadFromJsonAsync<RegisterUserResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }
}
