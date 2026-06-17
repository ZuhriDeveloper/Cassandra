using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Stock.ChangeStockStatus;
using Cassandra.Application.Queries.Stock;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/stock")]
public class StockController(
    ChangeStockStatusCommandHandler statusHandler,
    GetStocksQueryHandler queryHandler) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetStocksQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetStockByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-kios/{kiosId:guid}")]
    public async Task<IActionResult> GetByKios(Guid kiosId, CancellationToken ct)
    {
        var result = await queryHandler.HandleByKiosAsync(new GetStocksByKiosQuery(kiosId), ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStockStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new ChangeStockStatusCommand(id, request.Status, CurrentUser), ct);
        return NoContent();
    }
}

public record ChangeStockStatusRequest(string Status);
