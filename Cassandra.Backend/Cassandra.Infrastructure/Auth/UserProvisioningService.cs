using Cassandra.Application.Authorization;
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
        Guid? dealerId,
        CancellationToken ct = default)
    {
        // SuperAdmin is a platform role that is seeded only — never provisioned via the API.
        if (role == Roles.SuperAdmin)
            return new UserProvisioningResult(false, null, ["The SuperAdmin role cannot be provisioned."]);

        if (!await roleManager.RoleExistsAsync(role))
            return new UserProvisioningResult(false, null, [$"Role '{role}' does not exist."]);

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true,
            DealerId = dealerId,
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
