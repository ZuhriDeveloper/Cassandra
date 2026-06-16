using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.BiayaBiroJasa.CreateBiayaBiroJasa;
using Cassandra.Application.Commands.BiayaBiroJasa.SetBiayaBiroJasaItems;
using Cassandra.Application.Commands.BiayaBiroJasa.SetBiayaBiroJasaStatus;
using Cassandra.Application.Queries.BiayaBiroJasa;
using Cassandra.Domain.BiayaBiroJasa;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/biaya-biro-jasa")]
public class BiayaBiroJasaController(
    CreateBiayaBiroJasaCommandHandler createHandler,
    SetBiayaBiroJasaItemsCommandHandler itemsHandler,
    SetBiayaBiroJasaStatusCommandHandler statusHandler,
    GetBiayaBiroJasasQueryHandler queryHandler,
    IValidator<CreateBiayaBiroJasaCommand> createValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetBiayaBiroJasasQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetBiayaBiroJasaByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateBiayaBiroJasaRequest request, CancellationToken ct)
    {
        var command = new CreateBiayaBiroJasaCommand(request.SamsatId, request.BiroId, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPatch("{id:guid}/items")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetItems(Guid id, [FromBody] SetBiayaBiroJasaItemsRequest request, CancellationToken ct)
    {
        var items = request.Items
            .Select(x => new BiayaBiroJasaItemValue(x.TipeMotorId, x.BiayaStnk, x.Notice))
            .ToList();
        await itemsHandler.HandleAsync(new SetBiayaBiroJasaItemsCommand(id, items, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetBiayaBiroJasaStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetBiayaBiroJasaStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateBiayaBiroJasaRequest(Guid SamsatId, Guid BiroId);
public record SetBiayaBiroJasaItemsRequest(IReadOnlyList<BiayaBiroJasaItemRequest> Items);
public record BiayaBiroJasaItemRequest(Guid TipeMotorId, decimal BiayaStnk, decimal Notice);
public record SetBiayaBiroJasaStatusRequest(bool IsActive);
