using Cassandra.Application.Authorization;
using Cassandra.Application.Queries.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Finance;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/finance/finvoice")]
public class FInvoiceController(GetFInvoicesQueryHandler queryHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? trnType, CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetFInvoicesQuery(trnType), ct);
        return Ok(result);
    }
}
