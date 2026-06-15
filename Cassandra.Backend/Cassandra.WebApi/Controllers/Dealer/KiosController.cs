using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Kios.CreateKios;
using Cassandra.Application.Commands.Kios.SetKiosLimit;
using Cassandra.Application.Commands.Kios.SetKiosStatus;
using Cassandra.Application.Commands.Kios.UpdateKios;
using Cassandra.Application.Queries.Kios;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/kios")]
public class KiosController(
    CreateKiosCommandHandler       createHandler,
    UpdateKiosCommandHandler       updateHandler,
    SetKiosStatusCommandHandler    statusHandler,
    SetKiosLimitCommandHandler     limitHandler,
    GetKiosQueryHandler            queryHandler,
    IValidator<CreateKiosCommand>  createValidator,
    IValidator<UpdateKiosCommand>  updateValidator,
    IValidator<SetKiosLimitCommand> limitValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetKiosQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetKiosByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateKiosRequest request, CancellationToken ct)
    {
        var command = new CreateKiosCommand(
            request.Code, request.Name, request.Phone, request.Fax,
            request.Address ?? string.Empty, request.PicKaryawanId,
            request.Limit, CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateKiosRequest request, CancellationToken ct)
    {
        var command = new UpdateKiosCommand(
            id, request.Name, request.Phone, request.Fax,
            request.Address ?? string.Empty, request.PicKaryawanId, CurrentUser);

        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetKiosStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetKiosStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/limit")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetLimit(Guid id, [FromBody] SetKiosLimitRequest request, CancellationToken ct)
    {
        var command = new SetKiosLimitCommand(id, request.Limit, CurrentUser);
        var validation = await limitValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await limitHandler.HandleAsync(command, ct);
        return NoContent();
    }
}

public record CreateKiosRequest(
    string  Code,
    string  Name,
    string  Phone,
    string? Fax,
    string? Address,
    Guid    PicKaryawanId,
    decimal Limit);

public record UpdateKiosRequest(
    string  Name,
    string  Phone,
    string? Fax,
    string? Address,
    Guid    PicKaryawanId);

public record SetKiosStatusRequest(bool IsActive);
public record SetKiosLimitRequest(decimal Limit);
