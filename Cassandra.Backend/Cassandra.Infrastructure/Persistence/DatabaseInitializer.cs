using Cassandra.Application.Authorization;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Dealers;
using Cassandra.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cassandra.Infrastructure.Persistence;

public class DatabaseInitializer(
    AppDbContext dbContext,
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IDealerRepository dealerRepository,
    IDealerQueryRepository dealerQueryRepository,
    ILogger<DatabaseInitializer> logger)
{
    private const string SampleDealerName = "Dealer Pusat";
    private const string SampleDealerCode = "D1";

    // The sample dealer's own staff (Admin/Sales/Cashier). SuperAdmin is seeded separately.
    private static readonly SeedUser[] SeedUsers =
    [
        new("admin@cassandra.local",   Roles.Admin,   "Admin Cassandra",   "Admin@1234"),
        new("sales@cassandra.local",   Roles.Sales,   "Sales Cassandra",   "Sales@1234"),
        new("cashier@cassandra.local", Roles.Cashier, "Cashier Cassandra", "Cashier@1234"),
    ];

    public async Task InitialiseAsync(CancellationToken cancellationToken = default)
    {
        await MigrateAsync(cancellationToken);
        await SeedRolesAsync();
        await SeedSuperAdminAsync();

        var dealerId = await EnsureSampleDealerAsync(cancellationToken);
        await SeedDealerStaffAsync(dealerId);
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
        var roles = SeedUsers.Select(u => u.Role).Append(Roles.SuperAdmin).Distinct();
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

    /// <summary>
    /// Seeds the platform <c>SuperAdmin</c> — a user with NO dealer (DealerId = null) that
    /// registers dealers and provisions dealer-scoped users.
    /// </summary>
    private async Task SeedSuperAdminAsync()
    {
        const string email = "superadmin@cassandra.local";
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = "Super Admin",
            EmailConfirmed = true,
            DealerId = null,
        };

        var result = await userManager.CreateAsync(user, "SuperAdmin@1234");
        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to create SuperAdmin: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(user, Roles.SuperAdmin);
        logger.LogInformation("SuperAdmin '{Email}' created.", email);
    }

    /// <summary>
    /// Ensures a sample dealer exists and returns its id. Idempotent: if the dealer already
    /// exists (matched by code) its id is returned without re-registering.
    /// </summary>
    private async Task<Guid> EnsureSampleDealerAsync(CancellationToken ct)
    {
        if (await dealerQueryRepository.CodeExistsAsync(SampleDealerCode, ct))
        {
            var existing = (await dealerQueryRepository.GetAllAsync(ct))
                .First(d => d.Code == SampleDealerCode);
            return existing.Id;
        }

        var dealer = Dealer.Register(SampleDealerName, SampleDealerCode, "superadmin@cassandra.local");
        await dealerRepository.SaveAsync(dealer, ct);
        logger.LogInformation("Sample dealer '{Name}' ({Code}) created with id {Id}.",
            SampleDealerName, SampleDealerCode, dealer.Id.Value);
        return dealer.Id.Value;
    }

    private async Task SeedDealerStaffAsync(Guid dealerId)
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
                DealerId = dealerId,
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
