using Cassandra.Application.Contracts.Auth;
using Cassandra.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Cassandra.Infrastructure.Auth;

public class UserProvisioningService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IUserProvisioningService
{
    public async Task<UserProvisioningResult> ProvisionAsync(
        string email,
        string fullName,
        string password,
        string role,
        CancellationToken ct = default)
    {
        if (!await roleManager.RoleExistsAsync(role))
            return new UserProvisioningResult(false, null, [$"Role '{role}' does not exist."]);

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true,
        };

        var create = await userManager.CreateAsync(user, password);
        if (!create.Succeeded)
            return new UserProvisioningResult(false, null,
                create.Errors.Select(e => e.Description).ToList());

        var addRole = await userManager.AddToRoleAsync(user, role);
        if (!addRole.Succeeded)
            return new UserProvisioningResult(false, user.Id,
                addRole.Errors.Select(e => e.Description).ToList());

        return new UserProvisioningResult(true, user.Id, []);
    }
}
