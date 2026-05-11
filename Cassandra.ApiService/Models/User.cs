namespace Cassandra.ApiService.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}
