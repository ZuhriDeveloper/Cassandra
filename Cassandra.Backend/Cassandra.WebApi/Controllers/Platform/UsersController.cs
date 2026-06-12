using Cassandra.Application.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Platform;

/// <summary>
/// User provisioning. Only an Admin can create staff accounts (Sales / Cashier).
/// </summary>
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/platform/users")]
public class UsersController(IUserProvisioningService provisioning) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProvisionUserRequest request, CancellationToken ct)
    {
        var result = await provisioning.ProvisionAsync(
            request.Email, request.FullName, request.Password, request.Role, ct);

        return result.Succeeded
            ? Ok(new { id = result.UserId })
            : BadRequest(new { errors = result.Errors });
    }
}

public record ProvisionUserRequest(
    string Email,
    string FullName,
    string Password,
    string Role);
