using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.ApTransaction.RecordApPayment;
using Cassandra.Application.Queries.ApTransaction;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Finance;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/finance/ap-transactions")]
public class ApTransactionController(
    RecordApPaymentCommandHandler recordPaymentHandler,
    GetApTransactionsQueryHandler queryHandler,
    IValidator<RecordApPaymentCommand> recordPaymentValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetApTransactionsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetApTransactionByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:guid}/pay")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Cashier}")]
    public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordApPaymentRequest request, CancellationToken ct)
    {
        var command = new RecordApPaymentCommand(id, request.Amount, request.PaymentDate, request.PaymentMethod, CurrentUser);
        var validation = await recordPaymentValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var fInvoiceId = await recordPaymentHandler.HandleAsync(command, ct);
        return Ok(new { fInvoiceId });
    }
}

public record RecordApPaymentRequest(decimal Amount, DateTime PaymentDate, string PaymentMethod);
