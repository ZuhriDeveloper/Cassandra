namespace Cassandra.Application.Contracts.Auth;

/// <summary>
/// Builds the absolute Web UI link embedded in password-reset emails. The base URL
/// points at the Blazor app, not the API.
/// </summary>
public interface IAccountLinkBuilder
{
    string BuildPasswordResetLink(string email, string encodedToken);
}
