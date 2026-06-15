using Cassandra.Application.Authorization;
using Cassandra.Application.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Platform;

/// <summary>
/// Platform user provisioning. Only a <see cref="Roles.SuperAdmin"/> can create a dealer's
/// users (Admin / Sales / Cashier), assigning the target dealer explicitly.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.SuperAdmin)]
[Route("api/platform/users")]
public class UsersController(IUserProvisioningService provisioning) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProvisionUserRequest request, CancellationToken ct)
    {
        if (!Roles.IsProvisionable(request.Role))
            return BadRequest(new { errors = new[] { $"Role '{request.Role}' cannot be assigned." } });

        if (request.DealerId == Guid.Empty)
            return BadRequest(new { errors = new[] { "A dealer must be specified." } });

        var result = await provisioning.ProvisionAsync(
            request.Email, request.FullName, request.Password, request.Role, request.DealerId, ct);

        return result.Succeeded
            ? Ok(new { id = result.UserId })
            : BadRequest(new { errors = result.Errors });
    }
}

public record ProvisionUserRequest(
    string Email,
    string FullName,
    string Password,
    string Role,
    Guid DealerId);
