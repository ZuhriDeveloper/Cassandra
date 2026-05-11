using Cassandra.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.ApiService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();
            entity.HasData(
                new Role { Id = 1, Name = "Admin", Description = "Full system access" },
                new Role { Id = 2, Name = "Manager", Description = "Managerial access" },
                new Role { Id = 3, Name = "Employee", Description = "Standard employee access" }
            );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasOne(u => u.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(u => u.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(u => u.Employee)
                  .WithOne(e => e.User)
                  .HasForeignKey<User>(u => u.EmployeeId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
