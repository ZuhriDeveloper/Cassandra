using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.SoRetur.CreateSoRetur;
using Cassandra.Application.Queries.SoRetur;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/so-retur")]
public class SoReturController(
    CreateSoReturCommandHandler createHandler,
    GetSoRetursQueryHandler queryHandler,
    IValidator<CreateSoReturCommand> createValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetSoRetursQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetSoReturByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Create([FromBody] CreateSoReturRequest request, CancellationToken ct)
    {
        var command = new CreateSoReturCommand(
            request.ReturNumber,
            request.SoId,
            request.ReturDate,
            request.Reason,
            request.Items.Select(i => new CreateSoReturItemRequest(i.TipeMotorId, i.WarnaId, i.Qty, i.NettPrice)).ToList(),
            CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }
}

public record CreateSoReturItemBody(Guid TipeMotorId, Guid WarnaId, int Qty, decimal NettPrice);
public record CreateSoReturRequest(
    string ReturNumber,
    Guid SoId,
    DateOnly ReturDate,
    string Reason,
    List<CreateSoReturItemBody> Items);
