using Microsoft.AspNetCore.Identity;

namespace Cassandra.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }

    /// <summary>
    /// The dealer this user belongs to. <c>null</c> for a platform <c>SuperAdmin</c>, who is
    /// not bound to a single dealer and instead manages dealers and provisions users.
    /// </summary>
    public Guid? DealerId { get; set; }
}
