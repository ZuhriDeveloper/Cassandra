using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.PelanggaranWilayah.CreatePelanggaranWilayah;
using Cassandra.Application.Commands.PelanggaranWilayah.SetPelanggaranWilayahStatus;
using Cassandra.Application.Commands.PelanggaranWilayah.UpdatePelanggaranWilayah;
using Cassandra.Application.Queries.PelanggaranWilayah;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/pelanggaran-wilayah")]
public class PelanggaranWilayahController(
    CreatePelanggaranWilayahCommandHandler createHandler,
    UpdatePelanggaranWilayahCommandHandler updateHandler,
    SetPelanggaranWilayahStatusCommandHandler statusHandler,
    GetPelanggaranWilayahsQueryHandler queryHandler,
    IValidator<CreatePelanggaranWilayahCommand> createValidator,
    IValidator<UpdatePelanggaranWilayahCommand> updateValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetPelanggaranWilayahsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetPelanggaranWilayahByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreatePelanggaranWilayahRequest request, CancellationToken ct)
    {
        var command = new CreatePelanggaranWilayahCommand(request.AreaCode, request.ExtraFee, CurrentUser);
        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePelanggaranWilayahRequest request, CancellationToken ct)
    {
        var command = new UpdatePelanggaranWilayahCommand(id, request.ExtraFee, CurrentUser);
        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetPelanggaranWilayahStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetPelanggaranWilayahStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }
}

public record CreatePelanggaranWilayahRequest(string AreaCode, decimal ExtraFee);
public record UpdatePelanggaranWilayahRequest(decimal ExtraFee);
public record SetPelanggaranWilayahStatusRequest(bool IsActive);
