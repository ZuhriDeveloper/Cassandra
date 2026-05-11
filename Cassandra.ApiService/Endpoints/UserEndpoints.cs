using Cassandra.ApiService.Data;
using Cassandra.ApiService.DTOs;
using Cassandra.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.ApiService.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithOpenApi();

        group.MapPost("/", async (RegisterUserRequest request, AppDbContext db) =>
        {
            if (await db.Users.AnyAsync(u => u.Username == request.Username))
                return Results.Conflict(new { message = "Username is already taken." });

            var role = await db.Roles.FindAsync(request.RoleId);
            if (role is null)
                return Results.BadRequest(new { message = "Selected role does not exist." });

            if (request.EmployeeId.HasValue)
            {
                var employeeExists = await db.Employees.AnyAsync(e => e.Id == request.EmployeeId);
                if (!employeeExists)
                    return Results.BadRequest(new { message = "Selected employee does not exist." });

                var alreadyLinked = await db.Users.AnyAsync(u => u.EmployeeId == request.EmployeeId);
                if (alreadyLinked)
                    return Results.Conflict(new { message = "This employee already has a user account." });
            }

            var user = new User
            {
                Username     = request.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, 11),
                RoleId       = request.RoleId,
                EmployeeId   = request.EmployeeId,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/users/{user.Id}",
                new RegisterUserResponse(user.Id, user.Username, role.Name));
        })
        .WithName("RegisterUser")
        .AllowAnonymous();
    }
}
