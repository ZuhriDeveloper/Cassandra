using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.DaftarHargaLeasing.CreateDaftarHargaLeasing;
using Cassandra.Application.Commands.DaftarHargaLeasing.SetDaftarHargaLeasingItems;
using Cassandra.Application.Commands.DaftarHargaLeasing.SetDaftarHargaLeasingStatus;
using Cassandra.Application.Commands.DaftarHargaLeasing.UpdateDaftarHargaLeasing;
using Cassandra.Application.Queries.DaftarHargaLeasing;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/daftar-harga-leasing")]
public class DaftarHargaLeasingController(
    CreateDaftarHargaLeasingCommandHandler createHandler,
    UpdateDaftarHargaLeasingCommandHandler updateHandler,
    SetDaftarHargaLeasingStatusCommandHandler statusHandler,
    SetDaftarHargaLeasingItemsCommandHandler itemsHandler,
    GetDaftarHargaLeasingsQueryHandler queryHandler,
    IValidator<CreateDaftarHargaLeasingCommand> createValidator,
    IValidator<UpdateDaftarHargaLeasingCommand> updateValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetDaftarHargaLeasingsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetDaftarHargaLeasingByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateDaftarHargaLeasingRequest request, CancellationToken ct)
    {
        var command = new CreateDaftarHargaLeasingCommand(
            request.Name, request.GlobalLeasingId, request.GrupTenorId, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDaftarHargaLeasingRequest request, CancellationToken ct)
    {
        var command = new UpdateDaftarHargaLeasingCommand(
            id, request.Name, request.GlobalLeasingId, request.GrupTenorId, CurrentUser);
        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetDaftarHargaLeasingStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetDaftarHargaLeasingStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/items")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetItems(Guid id, [FromBody] SetDaftarHargaLeasingItemsRequest request, CancellationToken ct)
    {
        var items = (request.Items ?? [])
            .Select(i => new SetDaftarHargaLeasingItem(i.GrupTipeMotorId, i.Subsidi, i.Incentive, i.LainLain))
            .ToList();
        await itemsHandler.HandleAsync(new SetDaftarHargaLeasingItemsCommand(id, items, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateDaftarHargaLeasingRequest(string Name, Guid GlobalLeasingId, Guid GrupTenorId);
public record UpdateDaftarHargaLeasingRequest(string Name, Guid GlobalLeasingId, Guid GrupTenorId);
public record SetDaftarHargaLeasingStatusRequest(bool IsActive);
public record SetDaftarHargaLeasingItemsRequest(List<DaftarHargaLeasingItemInput>? Items);
public record DaftarHargaLeasingItemInput(Guid GrupTipeMotorId, decimal Subsidi, decimal Incentive, decimal LainLain);
