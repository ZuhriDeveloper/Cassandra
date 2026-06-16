using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Tenor.CreateTenor;
using Cassandra.Application.Commands.Tenor.SetTenorStatus;
using Cassandra.Application.Commands.Tenor.UpdateTenor;
using Cassandra.Application.Queries.Tenor;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/tenor")]
public class TenorController(
    CreateTenorCommandHandler createHandler,
    UpdateTenorCommandHandler updateHandler,
    SetTenorStatusCommandHandler statusHandler,
    GetTenorsQueryHandler queryHandler,
    IValidator<CreateTenorCommand> createValidator,
    IValidator<UpdateTenorCommand> updateValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetTenorsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetTenorByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateTenorRequest request, CancellationToken ct)
    {
        var command = new CreateTenorCommand(request.Code, request.Name, request.Months, request.GrupTenorId, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTenorRequest request, CancellationToken ct)
    {
        var command = new UpdateTenorCommand(id, request.Name, request.Months, request.GrupTenorId, CurrentUser);
        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetTenorStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetTenorStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateTenorRequest(string Code, string Name, int Months, Guid GrupTenorId);
public record UpdateTenorRequest(string Name, int Months, Guid GrupTenorId);
public record SetTenorStatusRequest(bool IsActive);
