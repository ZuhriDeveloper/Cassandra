using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.ArTransaction.RecordArPayment;
using Cassandra.Application.Queries.ArTransaction;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Finance;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/finance/ar-transactions")]
public class ArTransactionController(
    RecordArPaymentCommandHandler recordPaymentHandler,
    GetArTransactionsQueryHandler queryHandler,
    IValidator<RecordArPaymentCommand> recordPaymentValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetArTransactionsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetArTransactionByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:guid}/pay")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Cashier}")]
    public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordArPaymentRequest request, CancellationToken ct)
    {
        var command = new RecordArPaymentCommand(id, request.Amount, request.PaymentDate, request.PaymentMethod, CurrentUser);
        var validation = await recordPaymentValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var fInvoiceId = await recordPaymentHandler.HandleAsync(command, ct);
        return Ok(new { fInvoiceId });
    }
}

public record RecordArPaymentRequest(decimal Amount, DateTime PaymentDate, string PaymentMethod);
