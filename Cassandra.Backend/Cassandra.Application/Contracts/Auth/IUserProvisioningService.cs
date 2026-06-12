namespace Cassandra.Application.Contracts.Auth;

/// <summary>
/// Provisions user accounts. Used by an Admin to create staff (Sales / Cashier) accounts.
/// </summary>
public interface IUserProvisioningService
{
    Task<UserProvisioningResult> ProvisionAsync(
        string email,
        string fullName,
        string password,
        string role,
        CancellationToken ct = default);
}

public record UserProvisioningResult(bool Succeeded, string? UserId, IReadOnlyList<string> Errors);
