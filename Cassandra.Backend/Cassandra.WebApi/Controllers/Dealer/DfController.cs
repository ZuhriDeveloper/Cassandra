using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Df.SetDf;
using Cassandra.Application.Queries.Df;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/df")]
public class DfController(
    SetDfCommandHandler setHandler,
    GetDfQueryHandler queryHandler,
    IValidator<SetDfCommand> setValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetDfQuery(), ct);
        return result is null ? NoContent() : Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Set([FromBody] SetDfRequest request, CancellationToken ct)
    {
        var command = new SetDfCommand(request.Discount, request.Interest, request.StartDate, CurrentUser);
        var validation = await setValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await setHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }
}

public record SetDfRequest(decimal Discount, decimal Interest, DateOnly StartDate);
