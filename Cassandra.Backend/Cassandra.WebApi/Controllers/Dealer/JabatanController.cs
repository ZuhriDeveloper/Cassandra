using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Jabatan.CreateJabatan;
using Cassandra.Application.Commands.Jabatan.SetJabatanStatus;
using Cassandra.Application.Commands.Jabatan.UpdateJabatan;
using Cassandra.Application.Queries.Jabatan;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/jabatan")]
public class JabatanController(
    CreateJabatanCommandHandler createHandler,
    UpdateJabatanCommandHandler updateHandler,
    SetJabatanStatusCommandHandler statusHandler,
    GetJabatansQueryHandler queryHandler,
    IValidator<CreateJabatanCommand> createValidator,
    IValidator<UpdateJabatanCommand> updateValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetJabatansQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetJabatanByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateJabatanRequest request, CancellationToken ct)
    {
        var command = new CreateJabatanCommand(request.Name, request.Description ?? string.Empty, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateJabatanRequest request, CancellationToken ct)
    {
        var command = new UpdateJabatanCommand(id, request.Name, request.Description ?? string.Empty, CurrentUser);
        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetJabatanStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetJabatanStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateJabatanRequest(string Name, string? Description);
public record UpdateJabatanRequest(string Name, string? Description);
public record SetJabatanStatusRequest(bool IsActive);
