namespace Cassandra.ApiService.DTOs;

public record RegisterEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber);

public record RegisterEmployeeResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber);

public record RegisterUserRequest(
    string Username,
    string Password,
    int RoleId,
    int? EmployeeId);

public record RegisterUserResponse(
    int Id,
    string Username,
    string Role);

public record RoleDto(int Id, string Name);

public record EmployeeDto(int Id, string FullName, string Email);
