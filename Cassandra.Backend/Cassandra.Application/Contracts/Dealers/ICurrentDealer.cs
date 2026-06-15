namespace Cassandra.Application.Contracts.Dealers;

/// <summary>
/// Resolves the dealer (tenant) the current operation belongs to. In a request this comes
/// from the <c>dealerId</c> JWT claim; in background/seed work it comes from an ambient
/// scope. A platform <c>SuperAdmin</c> has no single dealer.
/// </summary>
public interface ICurrentDealer
{
    /// <summary>The current dealer. Throws when there is no dealer scope (write path).</summary>
    Guid DealerId { get; }

    /// <summary>The current dealer, or <c>null</c> for a platform admin / unscoped context.</summary>
    Guid? DealerIdOrNull { get; }

    /// <summary>True when the caller is a platform-level admin not bound to a single dealer.</summary>
    bool IsSuperAdmin { get; }
}
