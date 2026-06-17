using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.SoPenerimaan.CreateSoPenerimaan;
using Cassandra.Application.Queries.SoPenerimaan;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/so-penerimaan")]
public class SoPenerimaanController(
    CreateSoPenerimaanCommandHandler createHandler,
    GetSoPenerimaansQueryHandler queryHandler,
    IValidator<CreateSoPenerimaanCommand> createValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetSoPenerimaansQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetSoPenerimaanByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Create([FromBody] CreateSoPenerimaanRequest request, CancellationToken ct)
    {
        var command = new CreateSoPenerimaanCommand(
            request.SuratJalanId,
            request.SuratJalanDate,
            request.SoId,
            request.MotorItems.Select(m => new CreateSoPenerimaanItemMotorRequest(
                m.TipeMotorId, m.WarnaId, m.NoMesin, m.NoRangka, m.KiosId, m.AssemblyYear)).ToList(),
            request.KelengkapanItems.Select(k => new CreateSoPenerimaanItemKelengkapanRequest(
                k.KelengkapanId, k.Qty, k.Notes)).ToList(),
            CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }
}

public record CreateSoPenerimaanMotorItemBody(Guid TipeMotorId, Guid WarnaId, string NoMesin, string NoRangka, Guid KiosId, string AssemblyYear);
public record CreateSoPenerimaanKelengkapanItemBody(Guid KelengkapanId, int Qty, string? Notes);
public record CreateSoPenerimaanRequest(
    string SuratJalanId,
    DateOnly SuratJalanDate,
    Guid SoId,
    List<CreateSoPenerimaanMotorItemBody> MotorItems,
    List<CreateSoPenerimaanKelengkapanItemBody> KelengkapanItems);
