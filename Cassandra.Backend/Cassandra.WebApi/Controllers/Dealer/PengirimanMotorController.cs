using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.PengirimanMotor.CreatePengirimanMotor;
using Cassandra.Application.Queries.PengirimanMotor;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/pengiriman-motor")]
public class PengirimanMotorController(
    CreatePengirimanMotorCommandHandler          createHandler,
    GetPengirimanMotorsQueryHandler              queryHandler,
    IValidator<CreatePengirimanMotorCommand>     createValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetPengirimanMotorsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetPengirimanMotorByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Create([FromBody] CreatePengirimanMotorRequest request, CancellationToken ct)
    {
        var command = new CreatePengirimanMotorCommand(
            request.RegistrasiPenjualanId,
            request.NoMesin,
            request.Driver1Id,
            request.Driver2Id,
            request.DeliveryDate,
            request.Zona,
            CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }
}

public record CreatePengirimanMotorRequest(
    Guid     RegistrasiPenjualanId,
    string   NoMesin,
    Guid     Driver1Id,
    Guid?    Driver2Id,
    DateOnly DeliveryDate,
    string?  Zona);
