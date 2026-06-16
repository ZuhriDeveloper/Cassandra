using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Discount.CreateDiscount;
using Cassandra.Application.Commands.Discount.SetDiscountItems;
using Cassandra.Application.Commands.Discount.SetDiscountStatus;
using Cassandra.Application.Commands.Discount.UpdateDiscount;
using Cassandra.Application.Queries.Discount;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/discount")]
public class DiscountController(
    CreateDiscountCommandHandler createHandler,
    UpdateDiscountCommandHandler updateHandler,
    SetDiscountStatusCommandHandler statusHandler,
    SetDiscountItemsCommandHandler itemsHandler,
    GetDiscountsQueryHandler queryHandler,
    IValidator<CreateDiscountCommand> createValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetDiscountsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetDiscountByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateDiscountRequest request, CancellationToken ct)
    {
        var command = new CreateDiscountCommand(request.DaftarHargaLeasingId, request.Level, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetDiscountStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetDiscountStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/items")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetItems(Guid id, [FromBody] SetDiscountItemsRequest request, CancellationToken ct)
    {
        var items = (request.Items ?? [])
            .Select(i => new SetDiscountItem(i.GrupTipeMotorId, i.Amount))
            .ToList();
        await itemsHandler.HandleAsync(new SetDiscountItemsCommand(id, items, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateDiscountRequest(Guid DaftarHargaLeasingId, string Level);
public record SetDiscountStatusRequest(bool IsActive);
public record SetDiscountItemsRequest(List<DiscountItemInput>? Items);
public record DiscountItemInput(Guid GrupTipeMotorId, decimal Amount);
