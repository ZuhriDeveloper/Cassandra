using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Samsat.CreateSamsat;
using Cassandra.Application.Commands.Samsat.SetSamsatCities;
using Cassandra.Application.Commands.Samsat.SetSamsatStatus;
using Cassandra.Application.Commands.Samsat.UpdateSamsat;
using Cassandra.Application.Queries.Samsat;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/samsat")]
public class SamsatController(
    CreateSamsatCommandHandler createHandler,
    UpdateSamsatCommandHandler updateHandler,
    SetSamsatStatusCommandHandler statusHandler,
    SetSamsatCitiesCommandHandler citiesHandler,
    GetSamsatsQueryHandler queryHandler,
    IValidator<CreateSamsatCommand> createValidator,
    IValidator<UpdateSamsatCommand> updateValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetSamsatsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetSamsatByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateSamsatRequest request, CancellationToken ct)
    {
        var command = new CreateSamsatCommand(request.Name, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSamsatRequest request, CancellationToken ct)
    {
        var command = new UpdateSamsatCommand(id, request.Name, CurrentUser);
        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetSamsatStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetSamsatStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/cities")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetCities(Guid id, [FromBody] SetSamsatCitiesRequest request, CancellationToken ct)
    {
        await citiesHandler.HandleAsync(new SetSamsatCitiesCommand(id, request.Cities, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateSamsatRequest(string Name);
public record UpdateSamsatRequest(string Name);
public record SetSamsatStatusRequest(bool IsActive);
public record SetSamsatCitiesRequest(IReadOnlyList<string> Cities);
