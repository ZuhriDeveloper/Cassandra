using System.Net.Http.Json;

namespace Cassandra.Web.Services;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string Username, string Role, DateTime ExpiresAt);

public class AuthService(HttpClient httpClient)
{
    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(username, password));

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }
}
