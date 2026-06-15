using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.TipeMotor.CreateTipeMotor;
using Cassandra.Application.Commands.TipeMotor.SetTipeMotorColors;
using Cassandra.Application.Commands.TipeMotor.SetTipeMotorStatus;
using Cassandra.Application.Commands.TipeMotor.UpdateTipeMotor;
using Cassandra.Application.Queries.TipeMotor;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/tipe-motor")]
public class TipeMotorController(
    CreateTipeMotorCommandHandler createHandler,
    UpdateTipeMotorCommandHandler updateHandler,
    SetTipeMotorStatusCommandHandler statusHandler,
    SetTipeMotorColorsCommandHandler colorsHandler,
    GetTipeMotorsQueryHandler queryHandler,
    IValidator<CreateTipeMotorCommand> createValidator,
    IValidator<UpdateTipeMotorCommand> updateValidator,
    IValidator<SetTipeMotorColorsCommand> colorsValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetTipeMotorsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetTipeMotorByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateTipeMotorRequest request, CancellationToken ct)
    {
        var command = new CreateTipeMotorCommand(
            request.Code,
            request.GrupTipeMotorId,
            request.ShortName,
            request.ProductCode ?? string.Empty,
            request.WmsCode ?? string.Empty,
            request.AhmCode ?? string.Empty,
            request.EngineNumberFormat ?? string.Empty,
            request.ChassisNumberFormat ?? string.Empty,
            request.NettPrice,
            request.OrJakarta,
            request.OrTangerang,
            request.BbnJakarta,
            request.BbnTangerang,
            CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTipeMotorRequest request, CancellationToken ct)
    {
        var command = new UpdateTipeMotorCommand(
            id,
            request.GrupTipeMotorId,
            request.ShortName,
            request.ProductCode ?? string.Empty,
            request.WmsCode ?? string.Empty,
            request.AhmCode ?? string.Empty,
            request.EngineNumberFormat ?? string.Empty,
            request.ChassisNumberFormat ?? string.Empty,
            request.NettPrice,
            request.OrJakarta,
            request.OrTangerang,
            request.BbnJakarta,
            request.BbnTangerang,
            CurrentUser);

        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetTipeMotorStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetTipeMotorStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/colors")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetColors(Guid id, [FromBody] SetTipeMotorColorsRequest request, CancellationToken ct)
    {
        var command = new SetTipeMotorColorsCommand(id, request.WarnaIds ?? [], CurrentUser);
        var validation = await colorsValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await colorsHandler.HandleAsync(command, ct);
        return NoContent();
    }
}

public record CreateTipeMotorRequest(
    string Code,
    Guid GrupTipeMotorId,
    string ShortName,
    string? ProductCode,
    string? WmsCode,
    string? AhmCode,
    string? EngineNumberFormat,
    string? ChassisNumberFormat,
    decimal NettPrice,
    decimal OrJakarta,
    decimal OrTangerang,
    decimal BbnJakarta,
    decimal BbnTangerang);

public record UpdateTipeMotorRequest(
    Guid GrupTipeMotorId,
    string ShortName,
    string? ProductCode,
    string? WmsCode,
    string? AhmCode,
    string? EngineNumberFormat,
    string? ChassisNumberFormat,
    decimal NettPrice,
    decimal OrJakarta,
    decimal OrTangerang,
    decimal BbnJakarta,
    decimal BbnTangerang);

public record SetTipeMotorStatusRequest(bool IsActive);
public record SetTipeMotorColorsRequest(List<Guid>? WarnaIds);
