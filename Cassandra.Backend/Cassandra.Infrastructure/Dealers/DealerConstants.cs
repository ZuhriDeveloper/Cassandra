namespace Cassandra.Infrastructure.Dealers;

/// <summary>
/// Well-known constants for the multi-tenant plumbing. (Role names live in
/// <c>Cassandra.Application.Authorization.Roles</c>, shared across layers.)
/// </summary>
public static class DealerConstants
{
    /// <summary>JWT claim carrying the dealer a user belongs to. Absent for a SuperAdmin.</summary>
    public const string DealerClaimType = "dealerId";
}
