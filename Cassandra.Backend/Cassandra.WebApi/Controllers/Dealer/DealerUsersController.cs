using Cassandra.Application.Authorization;
using Cassandra.Application.Contracts.Auth;
using Cassandra.Application.Contracts.Dealers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

/// <summary>
/// Dealer-scoped user provisioning. A dealer <see cref="Roles.Admin"/> creates their own
/// staff (Sales / Cashier only) for their own dealer — the dealer is taken from the caller's
/// token, never from the request, so an Admin cannot provision into another dealer or mint
/// another Admin / SuperAdmin.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.Admin)]
[Route("api/dealer/users")]
public class DealerUsersController(
    IUserProvisioningService provisioning,
    ICurrentDealer currentDealer) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProvisionDealerUserRequest request, CancellationToken ct)
    {
        if (!Roles.IsDealerStaff(request.Role))
            return BadRequest(new { errors = new[] { $"A dealer admin may only assign the {string.Join(" or ", Roles.DealerStaff)} role." } });

        if (currentDealer.DealerIdOrNull is not Guid dealerId)
            return Forbid();

        var result = await provisioning.ProvisionAsync(
            request.Email, request.FullName, request.Password, request.Role, dealerId, ct);

        return result.Succeeded
            ? Ok(new { id = result.UserId })
            : BadRequest(new { errors = result.Errors });
    }
}

public record ProvisionDealerUserRequest(
    string Email,
    string FullName,
    string Password,
    string Role);
