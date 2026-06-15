using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Mediator.CreateMediator;
using Cassandra.Application.Commands.Mediator.SetMediatorLimit;
using Cassandra.Application.Commands.Mediator.SetMediatorStatus;
using Cassandra.Application.Commands.Mediator.UpdateMediator;
using Cassandra.Application.Queries.Mediator;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/mediator")]
public class MediatorController(
    CreateMediatorCommandHandler       createHandler,
    UpdateMediatorCommandHandler       updateHandler,
    SetMediatorStatusCommandHandler    statusHandler,
    SetMediatorLimitCommandHandler     limitHandler,
    GetMediatorsQueryHandler           queryHandler,
    IValidator<CreateMediatorCommand>  createValidator,
    IValidator<UpdateMediatorCommand>  updateValidator,
    IValidator<SetMediatorLimitCommand> limitValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetMediatorsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetMediatorByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateMediatorRequest request, CancellationToken ct)
    {
        var command = new CreateMediatorCommand(
            request.Name, request.KaryawanId,
            request.Address ?? string.Empty, request.Limit, CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMediatorRequest request, CancellationToken ct)
    {
        var command = new UpdateMediatorCommand(
            id, request.Name, request.KaryawanId,
            request.Address ?? string.Empty, CurrentUser);

        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetMediatorStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetMediatorStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/limit")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetLimit(Guid id, [FromBody] SetMediatorLimitRequest request, CancellationToken ct)
    {
        var command = new SetMediatorLimitCommand(id, request.Limit, CurrentUser);
        var validation = await limitValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await limitHandler.HandleAsync(command, ct);
        return NoContent();
    }
}

public record CreateMediatorRequest(
    string  Name,
    Guid    KaryawanId,
    string? Address,
    decimal Limit);

public record UpdateMediatorRequest(
    string  Name,
    Guid    KaryawanId,
    string? Address);

public record SetMediatorStatusRequest(bool IsActive);
public record SetMediatorLimitRequest(decimal Limit);
