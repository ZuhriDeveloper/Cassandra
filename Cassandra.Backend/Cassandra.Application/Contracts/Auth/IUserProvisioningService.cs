namespace Cassandra.Application.Contracts.Auth;

/// <summary>
/// Provisions user accounts. A SuperAdmin provisions a dealer's users (assigning the dealer
/// explicitly); a dealer Admin provisions their own staff (the dealer comes from their token).
/// </summary>
public interface IUserProvisioningService
{
    Task<UserProvisioningResult> ProvisionAsync(
        string email,
        string fullName,
        string password,
        string role,
        Guid? dealerId,
        CancellationToken ct = default);
}

public record UserProvisioningResult(bool Succeeded, string? UserId, IReadOnlyList<string> Errors);
