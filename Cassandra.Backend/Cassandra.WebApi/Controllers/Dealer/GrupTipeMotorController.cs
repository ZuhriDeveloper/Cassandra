using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.GrupTipeMotor.CreateGrupTipeMotor;
using Cassandra.Application.Commands.GrupTipeMotor.SetGrupTipeMotorStatus;
using Cassandra.Application.Queries.GrupTipeMotor;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/grup-tipe-motor")]
public class GrupTipeMotorController(
    CreateGrupTipeMotorCommandHandler createHandler,
    SetGrupTipeMotorStatusCommandHandler statusHandler,
    GetGrupTipeMotorsQueryHandler queryHandler,
    IValidator<CreateGrupTipeMotorCommand> createValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetGrupTipeMotorsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetGrupTipeMotorByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateGrupTipeMotorRequest request, CancellationToken ct)
    {
        var command = new CreateGrupTipeMotorCommand(request.Code, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetGrupTipeMotorStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetGrupTipeMotorStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateGrupTipeMotorRequest(string Code);
public record SetGrupTipeMotorStatusRequest(bool IsActive);
