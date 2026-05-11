using Cassandra.ApiService.Data;
using Cassandra.ApiService.DTOs;
using Cassandra.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.ApiService.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/employees").WithOpenApi();

        group.MapGet("/", async (AppDbContext db) =>
        {
            var employees = await db.Employees
                .OrderBy(e => e.FirstName)
                .Select(e => new EmployeeDto(
                    e.Id,
                    $"{e.FirstName} {e.LastName}",
                    e.Email))
                .ToListAsync();

            return Results.Ok(employees);
        })
        .WithName("GetEmployees")
        .AllowAnonymous();

        group.MapPost("/", async (RegisterEmployeeRequest request, AppDbContext db) =>
        {
            if (await db.Employees.AnyAsync(e => e.Email == request.Email))
                return Results.Conflict(new { message = "Email is already registered." });

            var employee = new Employee
            {
                FirstName = request.FirstName.Trim(),
                LastName  = request.LastName.Trim(),
                Email     = request.Email.Trim().ToLowerInvariant(),
                PhoneNumber = request.PhoneNumber?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            db.Employees.Add(employee);
            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/employees/{employee.Id}",
                new RegisterEmployeeResponse(
                    employee.Id,
                    employee.FirstName,
                    employee.LastName,
                    employee.Email,
                    employee.PhoneNumber));
        })
        .WithName("RegisterEmployee")
        .AllowAnonymous();
    }
}
