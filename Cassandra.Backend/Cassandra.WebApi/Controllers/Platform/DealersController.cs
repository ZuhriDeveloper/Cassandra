using System.Security.Claims;
using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Dealers.RegisterDealer;
using Cassandra.Application.Commands.Dealers.RenameDealer;
using Cassandra.Application.Commands.Dealers.SetDealerStatus;
using Cassandra.Application.Queries.Dealers;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Platform;

/// <summary>
/// Dealer registry. Only a platform <see cref="Roles.SuperAdmin"/> can register or manage dealers.
/// </summary>
[ApiController]
[Authorize(Roles = Roles.SuperAdmin)]
[Route("api/platform/dealers")]
public class DealersController(
    RegisterDealerCommandHandler registerHandler,
    RenameDealerCommandHandler renameHandler,
    SetDealerStatusCommandHandler statusHandler,
    GetDealersQueryHandler queryHandler,
    IValidator<RegisterDealerCommand> registerValidator,
    IValidator<RenameDealerCommand> renameValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirstValue("fullName") is { Length: > 0 } fn ? fn :
        User.FindFirstValue(ClaimTypes.Email) ??
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await queryHandler.HandleAsync(new GetDealersQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetDealerByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterDealerRequest request, CancellationToken ct)
    {
        var command = new RegisterDealerCommand(request.Name, request.Code, CurrentUser);
        var validation = await registerValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await registerHandler.HandleAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}/name")]
    public async Task<IActionResult> Rename(Guid id, [FromBody] RenameDealerRequest request, CancellationToken ct)
    {
        var command = new RenameDealerCommand(id, request.Name, CurrentUser);
        var validation = await renameValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await renameHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetDealerStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetDealerStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record RegisterDealerRequest(string Name, string Code);
public record RenameDealerRequest(string Name);
public record SetDealerStatusRequest(bool IsActive);
