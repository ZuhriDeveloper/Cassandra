using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Karyawan.CreateKaryawan;
using Cassandra.Application.Commands.Karyawan.RecordKaryawanResign;
using Cassandra.Application.Commands.Karyawan.SetKaryawanLimit;
using Cassandra.Application.Commands.Karyawan.SetKaryawanStatus;
using Cassandra.Application.Commands.Karyawan.UpdateKaryawan;
using Cassandra.Application.Queries.Karyawan;
using Cassandra.Domain.Karyawan;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/karyawan")]
public class KaryawanController(
    CreateKaryawanCommandHandler       createHandler,
    UpdateKaryawanCommandHandler       updateHandler,
    SetKaryawanStatusCommandHandler    statusHandler,
    SetKaryawanLimitCommandHandler     limitHandler,
    RecordKaryawanResignCommandHandler resignHandler,
    GetKaryawansQueryHandler           queryHandler,
    IValidator<CreateKaryawanCommand>  createValidator,
    IValidator<UpdateKaryawanCommand>  updateValidator,
    IValidator<SetKaryawanLimitCommand> limitValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetKaryawansQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetKaryawanByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateKaryawanRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<Gender>(request.Gender, ignoreCase: true, out var gender))
            return BadRequest(new { errors = new[] { "Jenis kelamin tidak valid." } });

        var command = new CreateKaryawanCommand(
            request.Name, request.Email, request.KtpNumber,
            gender, request.HireDate,
            request.Phone, request.PhoneAlt, request.Address ?? string.Empty,
            request.SalesLimit, request.JabatanId, CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateKaryawanRequest request, CancellationToken ct)
    {
        var command = new UpdateKaryawanCommand(
            id, request.Name, request.Email,
            request.Phone, request.PhoneAlt, request.Address ?? string.Empty,
            request.JabatanId, CurrentUser);

        var validation = await updateValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await updateHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetStatus(Guid id, [FromBody] SetKaryawanStatusRequest request, CancellationToken ct)
    {
        await statusHandler.HandleAsync(new SetKaryawanStatusCommand(id, request.IsActive, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/limit")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetLimit(Guid id, [FromBody] SetKaryawanLimitRequest request, CancellationToken ct)
    {
        var command = new SetKaryawanLimitCommand(id, request.SalesLimit, CurrentUser);
        var validation = await limitValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await limitHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/resign")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> RecordResign(Guid id, [FromBody] RecordKaryawanResignRequest request, CancellationToken ct)
    {
        await resignHandler.HandleAsync(new RecordKaryawanResignCommand(id, request.ResignDate, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateKaryawanRequest(
    string   Name,
    string   Email,
    string   KtpNumber,
    string   Gender,
    DateOnly HireDate,
    string   Phone,
    string?  PhoneAlt,
    string?  Address,
    decimal  SalesLimit,
    Guid     JabatanId);

public record UpdateKaryawanRequest(
    string   Name,
    string   Email,
    string   Phone,
    string?  PhoneAlt,
    string?  Address,
    Guid     JabatanId);

public record SetKaryawanStatusRequest(bool IsActive);
public record SetKaryawanLimitRequest(decimal SalesLimit);
public record RecordKaryawanResignRequest(DateOnly ResignDate);
