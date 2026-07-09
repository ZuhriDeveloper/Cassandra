using Cassandra.Application.Contracts.Auth;
using Microsoft.Extensions.Configuration;

namespace Cassandra.Infrastructure.Auth;

/// <summary>
/// Builds links into the Blazor Web UI (<c>WebUi:BaseUrl</c>) for account emails.
/// The same base URL is already used by CORS and by the API configuration.
/// </summary>
public class AccountLinkBuilder(IConfiguration configuration) : IAccountLinkBuilder
{
    private readonly string _baseUrl =
        (configuration["WebUi:BaseUrl"] ?? "https://localhost:7266").TrimEnd('/');

    public string BuildPasswordResetLink(string email, string encodedToken) =>
        $"{_baseUrl}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(encodedToken)}";
}
