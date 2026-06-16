using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.GlobalLeasing.CreateGlobalLeasing;
using Cassandra.Application.Commands.GlobalLeasing.SetGlobalLeasingStatus;
using Cassandra.Application.Commands.GlobalLeasing.UpdateGlobalLeasing;
using Cassandra.Application.Queries.GlobalLeasing;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/global-leasing")]
public class GlobalLeasingController(
    CreateGlobalLeasingCommandHandler createHandler,
    UpdateGlobalLeasingCommandHandler updateHandler,
    SetGlobalLeasingStatusCommandHandler statusHandler,
    GetGlobalLeasingsQueryHandler queryHandler,
    IValidator<CreateGlobalLeasingCommand> createValidator,
    IValidator<UpdateGlobalLeasingCommand> updateValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetGlobalLeasingsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetGlobalLeasingByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateGlobalLeasingRequest request, CancellationToken ct)
    {
        var command = new CreateGlobalLeasingCommand(
            request.Code, request.Name, request.Phone, request.Fax, request.Contact, request.Address, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGlobalLeasingRequest request, CancellationToken ct)
    {
        var command = new UpdateGlobalLeasingCommand(
            id, request.Name, request.Phone, request.Fax, request.Contact, request.Address, CurrentUser);
        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetGlobalLeasingStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetGlobalLeasingStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateGlobalLeasingRequest(string Code, string Name, string Phone, string? Fax, string Contact, string Address);
public record UpdateGlobalLeasingRequest(string Name, string Phone, string? Fax, string Contact, string Address);
public record SetGlobalLeasingStatusRequest(bool IsActive);
