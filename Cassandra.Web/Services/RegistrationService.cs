using System.Net.Http.Json;

namespace Cassandra.Web.Services;

public record RegisterEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber);

public record RegisterUserRequest(
    string Username,
    string Password,
    int RoleId,
    int? EmployeeId);

public record RoleDto(int Id, string Name);
public record EmployeeDto(int Id, string FullName, string Email);

public class RegistrationService(HttpClient httpClient)
{
    public async Task<(bool Success, string? Error)> RegisterEmployeeAsync(RegisterEmployeeRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("/api/employees", request);
        if (response.IsSuccessStatusCode) return (true, null);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetail>();
        return (false, problem?.Message ?? "Registration failed.");
    }

    public async Task<(bool Success, string? Error)> RegisterUserAsync(RegisterUserRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("/api/users", request);
        if (response.IsSuccessStatusCode) return (true, null);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetail>();
        return (false, problem?.Message ?? "Registration failed.");
    }

    public async Task<List<RoleDto>> GetRolesAsync()
    {
        try { return await httpClient.GetFromJsonAsync<List<RoleDto>>("/api/roles") ?? []; }
        catch { return []; }
    }

    public async Task<List<EmployeeDto>> GetEmployeesAsync()
    {
        try { return await httpClient.GetFromJsonAsync<List<EmployeeDto>>("/api/employees") ?? []; }
        catch { return []; }
    }

    private record ProblemDetail(string? Message);
}
