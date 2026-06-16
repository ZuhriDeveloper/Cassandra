using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.GrupTenor.CreateGrupTenor;
using Cassandra.Application.Commands.GrupTenor.SetGrupTenorStatus;
using Cassandra.Application.Commands.GrupTenor.UpdateGrupTenor;
using Cassandra.Application.Queries.GrupTenor;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/grup-tenor")]
public class GrupTenorController(
    CreateGrupTenorCommandHandler createHandler,
    UpdateGrupTenorCommandHandler updateHandler,
    SetGrupTenorStatusCommandHandler statusHandler,
    GetGrupTenorsQueryHandler queryHandler,
    IValidator<CreateGrupTenorCommand> createValidator,
    IValidator<UpdateGrupTenorCommand> updateValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetGrupTenorsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetGrupTenorByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateGrupTenorRequest request, CancellationToken ct)
    {
        var command = new CreateGrupTenorCommand(request.Code, request.Name, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGrupTenorRequest request, CancellationToken ct)
    {
        var command = new UpdateGrupTenorCommand(id, request.Name, CurrentUser);
        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetGrupTenorStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetGrupTenorStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateGrupTenorRequest(string Code, string Name);
public record UpdateGrupTenorRequest(string Name);
public record SetGrupTenorStatusRequest(bool IsActive);
