namespace Cassandra.Application.Authorization;

/// <summary>
/// Canonical role names and the rules for who may assign them. Defined here (Application)
/// so controllers, validators, the provisioning service, and tests share one source of truth.
/// The constants are usable in <c>[Authorize(Roles = ...)]</c> attributes.
/// </summary>
public static class Roles
{
    /// <summary>Platform-level admin, not bound to a dealer. Manages dealers and provisions users.</summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>Dealer-scoped admin. Manages their own dealer's staff.</summary>
    public const string Admin = "Admin";

    /// <summary>Dealer-scoped sales staff.</summary>
    public const string Sales = "Sales";

    /// <summary>Dealer-scoped cashier staff.</summary>
    public const string Cashier = "Cashier";

    /// <summary>Roles a SuperAdmin may assign when provisioning a dealer's users.</summary>
    public static readonly IReadOnlyList<string> Provisionable = [Admin, Sales, Cashier];

    /// <summary>Roles a dealer Admin may assign to their own staff.</summary>
    public static readonly IReadOnlyList<string> DealerStaff = [Sales, Cashier];

    /// <summary>
    /// True if <paramref name="role"/> may be assigned via the platform (SuperAdmin)
    /// provisioning endpoint. Excludes <see cref="SuperAdmin"/>, which is seeded only.
    /// </summary>
    public static bool IsProvisionable(string role) => Provisionable.Contains(role);

    /// <summary>True if a dealer Admin may assign <paramref name="role"/> to their own staff.</summary>
    public static bool IsDealerStaff(string role) => DealerStaff.Contains(role);
}
