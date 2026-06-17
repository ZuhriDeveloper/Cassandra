using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Bpkb.HandoverBpkb;
using Cassandra.Application.Commands.Bpkb.ReceiveBpkb;
using Cassandra.Application.Queries.Bpkb;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/bpkb")]
public class BpkbController(
    ReceiveBpkbCommandHandler        receiveHandler,
    HandoverBpkbCommandHandler       handoverHandler,
    GetBpkbsQueryHandler             queryHandler,
    IValidator<ReceiveBpkbCommand>   receiveValidator,
    IValidator<HandoverBpkbCommand>  handoverValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetBpkbsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetBpkbByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:guid}/receive")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Receive(Guid id, [FromBody] ReceiveBpkbRequest request, CancellationToken ct)
    {
        var command = new ReceiveBpkbCommand(
            id,
            request.ReceiveDate,
            request.BpkbNumber,
            request.BookNumber,
            CurrentUser);

        var validation = await receiveValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await receiveHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/handover")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Handover(Guid id, [FromBody] HandoverBpkbRequest request, CancellationToken ct)
    {
        var command = new HandoverBpkbCommand(
            id,
            request.HandoverDate,
            request.Receiver,
            CurrentUser);

        var validation = await handoverValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await handoverHandler.HandleAsync(command, ct);
        return NoContent();
    }
}

public record ReceiveBpkbRequest(
    DateOnly ReceiveDate,
    string   BpkbNumber,
    string   BookNumber);

public record HandoverBpkbRequest(DateOnly HandoverDate, string Receiver);
