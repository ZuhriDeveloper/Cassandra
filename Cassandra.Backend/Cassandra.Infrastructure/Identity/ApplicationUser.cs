using Microsoft.AspNetCore.Identity;

namespace Cassandra.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
