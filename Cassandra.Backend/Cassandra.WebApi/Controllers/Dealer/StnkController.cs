using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.Stnk.CreateStnk;
using Cassandra.Application.Commands.Stnk.HandoverStnk;
using Cassandra.Application.Commands.Stnk.ProcessStnk;
using Cassandra.Application.Commands.Stnk.ReceiveStnk;
using Cassandra.Application.Queries.Stnk;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/stnk")]
public class StnkController(
    CreateStnkCommandHandler             createHandler,
    ProcessStnkCommandHandler            processHandler,
    ReceiveStnkCommandHandler            receiveHandler,
    HandoverStnkCommandHandler           handoverHandler,
    GetStnksQueryHandler                 queryHandler,
    IValidator<CreateStnkCommand>        createValidator,
    IValidator<ProcessStnkCommand>       processValidator,
    IValidator<ReceiveStnkCommand>       receiveValidator,
    IValidator<HandoverStnkCommand>      handoverValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetStnksQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetStnkByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Create([FromBody] CreateStnkRequest request, CancellationToken ct)
    {
        var command = new CreateStnkCommand(
            request.RegistrasiPenjualanId,
            request.FakturDate,
            request.FakturName,
            request.FakturAddress,
            CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPatch("{id:guid}/process")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Process(Guid id, [FromBody] ProcessStnkRequest request, CancellationToken ct)
    {
        var command = new ProcessStnkCommand(
            id,
            request.ProcessDate,
            request.BiroId,
            request.InvoiceNumber,
            CurrentUser);

        var validation = await processValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await processHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/receive")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Receive(Guid id, [FromBody] ReceiveStnkRequest request, CancellationToken ct)
    {
        var command = new ReceiveStnkCommand(
            id,
            request.ReceiveDate,
            request.PlateNumber,
            request.BiroId,
            request.StnkNumber,
            request.StnkCost,
            request.NoticeCost,
            request.ProgressiveCost,
            request.InvoiceNumber,
            request.Region,
            request.BbnCost,
            request.PnbpCost,
            request.AdminCost,
            request.OtherCost,
            request.ServiceCost,
            request.PphCost,
            request.IsInvoiceValid,
            CurrentUser);

        var validation = await receiveValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await receiveHandler.HandleAsync(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/handover")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Handover(Guid id, [FromBody] HandoverStnkRequest request, CancellationToken ct)
    {
        var command = new HandoverStnkCommand(
            id,
            request.HandoverDate,
            request.StnkReceiver,
            CurrentUser);

        var validation = await handoverValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        await handoverHandler.HandleAsync(command, ct);
        return NoContent();
    }
}

public record CreateStnkRequest(
    Guid     RegistrasiPenjualanId,
    DateOnly FakturDate,
    string   FakturName,
    string   FakturAddress);

public record ProcessStnkRequest(
    DateOnly ProcessDate,
    Guid     BiroId,
    string   InvoiceNumber);

public record ReceiveStnkRequest(
    DateOnly ReceiveDate,
    string   PlateNumber,
    Guid     BiroId,
    string   StnkNumber,
    decimal  StnkCost,
    decimal  NoticeCost,
    decimal  ProgressiveCost,
    string   InvoiceNumber,
    string?  Region,
    decimal  BbnCost,
    decimal  PnbpCost,
    decimal  AdminCost,
    decimal  OtherCost,
    decimal  ServiceCost,
    decimal  PphCost,
    bool     IsInvoiceValid);

public record HandoverStnkRequest(DateOnly HandoverDate, string StnkReceiver);
