using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.CabangLeasing.CreateCabangLeasing;
using Cassandra.Application.Commands.CabangLeasing.SetCabangLeasingStatus;
using Cassandra.Application.Commands.CabangLeasing.UpdateCabangLeasing;
using Cassandra.Application.Queries.CabangLeasing;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/cabang-leasing")]
public class CabangLeasingController(
    CreateCabangLeasingCommandHandler createHandler,
    UpdateCabangLeasingCommandHandler updateHandler,
    SetCabangLeasingStatusCommandHandler statusHandler,
    GetCabangLeasingsQueryHandler queryHandler,
    IValidator<CreateCabangLeasingCommand> createValidator,
    IValidator<UpdateCabangLeasingCommand> updateValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetCabangLeasingsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetCabangLeasingByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateCabangLeasingRequest request, CancellationToken ct)
    {
        var command = new CreateCabangLeasingCommand(
            request.Code, request.Name, request.Phone, request.Fax, request.Contact, request.GlobalLeasingId, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCabangLeasingRequest request, CancellationToken ct)
    {
        var command = new UpdateCabangLeasingCommand(
            id, request.Name, request.Phone, request.Fax, request.Contact, request.GlobalLeasingId, CurrentUser);
        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetCabangLeasingStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetCabangLeasingStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateCabangLeasingRequest(string Code, string Name, string? Phone, string? Fax, string? Contact, Guid GlobalLeasingId);
public record UpdateCabangLeasingRequest(string Name, string? Phone, string? Fax, string? Contact, Guid GlobalLeasingId);
public record SetCabangLeasingStatusRequest(bool IsActive);
