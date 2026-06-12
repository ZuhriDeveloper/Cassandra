using Cassandra.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cassandra.Infrastructure.Persistence;

public class DatabaseInitializer(
    AppDbContext dbContext,
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ILogger<DatabaseInitializer> logger)
{
    private static readonly SeedUser[] SeedUsers =
    [
        new("admin@cassandra.local",   "Admin",   "Admin Cassandra",   "Admin@1234"),
        new("sales@cassandra.local",   "Sales",   "Sales Cassandra",   "Sales@1234"),
        new("cashier@cassandra.local", "Cashier", "Cashier Cassandra", "Cashier@1234"),
    ];

    public async Task InitialiseAsync(CancellationToken cancellationToken = default)
    {
        await MigrateAsync(cancellationToken);
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task MigrateAsync(CancellationToken cancellationToken)
    {
        var pending = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (!pending.Any())
        {
            logger.LogInformation("No pending migrations.");
            return;
        }

        logger.LogInformation("Applying {Count} pending migration(s)…", pending.Count());
        await dbContext.Database.MigrateAsync(cancellationToken);
        logger.LogInformation("Migrations applied successfully.");
    }

    private async Task SeedRolesAsync()
    {
        var roles = SeedUsers.Select(u => u.Role).Distinct();
        foreach (var role in roles)
        {
            if (await roleManager.RoleExistsAsync(role))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole(role));
            if (result.Succeeded)
                logger.LogInformation("Role '{Role}' created.", role);
            else
                logger.LogWarning("Failed to create role '{Role}': {Errors}", role,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private async Task SeedUsersAsync()
    {
        foreach (var seed in SeedUsers)
        {
            if (await userManager.FindByEmailAsync(seed.Email) is not null)
                continue;

            var user = new ApplicationUser
            {
                UserName = seed.Email,
                Email = seed.Email,
                FullName = seed.FullName,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(user, seed.Password);
            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to create user '{Email}': {Errors}", seed.Email,
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                continue;
            }

            await userManager.AddToRoleAsync(user, seed.Role);
            logger.LogInformation("User '{Email}' created and assigned to role '{Role}'.", seed.Email, seed.Role);
        }
    }

    private record SeedUser(string Email, string Role, string FullName, string Password);
}
