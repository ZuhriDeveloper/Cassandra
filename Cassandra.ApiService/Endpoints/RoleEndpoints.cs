using Cassandra.ApiService.Data;
using Cassandra.ApiService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.ApiService.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this WebApplication app)
    {
        app.MapGet("/api/roles", async (AppDbContext db) =>
        {
            var roles = await db.Roles
                .OrderBy(r => r.Name)
                .Select(r => new RoleDto(r.Id, r.Name))
                .ToListAsync();

            return Results.Ok(roles);
        })
        .WithName("GetRoles")
        .WithOpenApi()
        .AllowAnonymous();
    }
}
