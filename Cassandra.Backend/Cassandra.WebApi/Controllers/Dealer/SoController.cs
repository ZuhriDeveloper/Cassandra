using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.So.ChangeSoStatus;
using Cassandra.Application.Commands.So.CreateSo;
using Cassandra.Application.Commands.So.DeleteSo;
using Cassandra.Application.Queries.So;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/so")]
public class SoController(
    CreateSoCommandHandler createHandler,
    ChangeSoStatusCommandHandler statusHandler,
    DeleteSoCommandHandler deleteHandler,
    GetSosQueryHandler queryHandler,
    IValidator<CreateSoCommand> createValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetSosQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetSoByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Create([FromBody] CreateSoRequest request, CancellationToken ct)
    {
        var command = new CreateSoCommand(
            request.SoNumber,
            request.SoDate,
            request.DueDate,
            request.PaymentType,
            request.MetodeKeuanganId,
            request.Subsidi,
            request.CashDiscount,
            request.Df,
            request.Items.Select(i => new CreateSoItemRequest(i.TipeMotorId, i.WarnaId, i.Qty, i.NettPrice)).ToList(),
            CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeSoStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new ChangeSoStatusCommand(id, request.Status, CurrentUser), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await deleteHandler.HandleAsync(new DeleteSoCommand(id, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateSoItemRequestBody(Guid TipeMotorId, Guid WarnaId, int Qty, decimal NettPrice);
public record CreateSoRequest(
    string SoNumber,
    DateOnly SoDate,
    DateOnly DueDate,
    string PaymentType,
    Guid MetodeKeuanganId,
    decimal Subsidi,
    decimal CashDiscount,
    decimal Df,
    List<CreateSoItemRequestBody> Items);
public record ChangeSoStatusRequest(string Status);
