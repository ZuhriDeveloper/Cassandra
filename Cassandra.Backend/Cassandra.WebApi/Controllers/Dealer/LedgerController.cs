using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Ledger.CreateLedger;
using Cassandra.Application.Commands.Ledger.SetLedgerStatus;
using Cassandra.Application.Commands.Ledger.UpdateLedger;
using Cassandra.Application.Queries.Ledger;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/ledger")]
public class LedgerController(
    CreateLedgerCommandHandler createHandler,
    UpdateLedgerCommandHandler updateHandler,
    SetLedgerStatusCommandHandler statusHandler,
    GetLedgersQueryHandler queryHandler,
    IValidator<CreateLedgerCommand> createValidator,
    IValidator<UpdateLedgerCommand> updateValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetLedgersQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetLedgerByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateLedgerRequest request, CancellationToken ct)
    {
        var command = new CreateLedgerCommand(request.Name, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLedgerRequest request, CancellationToken ct)
    {
        var command = new UpdateLedgerCommand(id, request.Name, CurrentUser);
        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetLedgerStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetLedgerStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateLedgerRequest(string Name);
public record UpdateLedgerRequest(string Name);
public record SetLedgerStatusRequest(bool IsActive);
