using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Mutasi.CreateMutasi;
using Cassandra.Application.Queries.Mutasi;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/mutasi")]
public class MutasiController(
    CreateMutasiCommandHandler createHandler,
    GetMutasisQueryHandler queryHandler,
    IValidator<CreateMutasiCommand> createValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetMutasisQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetMutasiByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Create([FromBody] CreateMutasiRequest request, CancellationToken ct)
    {
        var command = new CreateMutasiCommand(
            request.MutasiNumber,
            request.MutasiDate,
            request.SourceKiosId,
            request.DestinationKiosId,
            request.EngineNumbers,
            request.KelengkapanItems.Select(k => new CreateMutasiKelengkapanItemRequest(k.KelengkapanName, k.Qty)).ToList(),
            CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }
}

public record CreateMutasiKelengkapanItemBody(string KelengkapanName, int Qty);
public record CreateMutasiRequest(
    string MutasiNumber,
    DateOnly MutasiDate,
    Guid SourceKiosId,
    Guid DestinationKiosId,
    List<string> EngineNumbers,
    List<CreateMutasiKelengkapanItemBody> KelengkapanItems);
