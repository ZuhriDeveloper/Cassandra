using Cassandra.Application.Authorization;
using Cassandra.Application.Commands.RegistrasiPenjualan.ApproveRegistrasiPenjualan;
using Cassandra.Application.Commands.RegistrasiPenjualan.CreateRegistrasiPenjualan;
using Cassandra.Application.Commands.RegistrasiPenjualan.SetEnableToVoid;
using Cassandra.Application.Commands.RegistrasiPenjualan.VoidRegistrasiPenjualan;
using Cassandra.Application.Queries.RegistrasiPenjualan;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cassandra.WebApi.Controllers.Dealer;

[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Sales},{Roles.Cashier}")]
[Route("api/dealer/registrasi-penjualan")]
public class RegistrasiPenjualanController(
    CreateRegistrasiPenjualanCommandHandler   createHandler,
    ApproveRegistrasiPenjualanCommandHandler  approveHandler,
    VoidRegistrasiPenjualanCommandHandler     voidHandler,
    SetEnableToVoidCommandHandler             enableToVoidHandler,
    GetRegistrasiPenjualansQueryHandler       queryHandler,
    IValidator<CreateRegistrasiPenjualanCommand> createValidator) : ControllerBase
{
    private string CurrentUser =>
        User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User.Identity?.Name
        ?? "unknown";

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await queryHandler.HandleAsync(new GetRegistrasiPenjualansQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await queryHandler.HandleByIdAsync(new GetRegistrasiPenjualanByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Sales}")]
    public async Task<IActionResult> Create([FromBody] CreateRegistrasiPenjualanRequest request, CancellationToken ct)
    {
        var command = new CreateRegistrasiPenjualanCommand(
            request.NoPenjualan,
            request.SaleDate,
            request.KaryawanId,
            request.KiosId,
            request.MediatorId,
            request.MetodePenjualan,
            request.TipePenjualan,
            request.NoMesin,
            request.NoRangka,
            request.NamaCustomer,
            request.Address,
            request.Phone,
            request.Phone1,
            request.Phone2,
            request.OffRoad,
            request.Bbn,
            request.Discount,
            request.Total,
            request.AmbilUang,
            request.Dp,
            request.Angsuran,
            request.DaftarHargaLeasingId,
            request.TenorCode,
            request.SerahTerimaKendaraanId,
            request.TandaTerimaSementaraId,
            request.Kelengkapan,
            CurrentUser);

        var validation = await createValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

        var id = await createHandler.HandleAsync(command, ct);
        return Ok(new { id });
    }

    [HttpPatch("{id:guid}/approve")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveRegistrasiPenjualanRequest request, CancellationToken ct)
    {
        await approveHandler.HandleAsync(new ApproveRegistrasiPenjualanCommand(id, request.ApprovedDiscount, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/void")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Void(Guid id, CancellationToken ct)
    {
        await voidHandler.HandleAsync(new VoidRegistrasiPenjualanCommand(id, CurrentUser), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/enable-to-void")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> SetEnableToVoid(Guid id, [FromBody] SetEnableToVoidRequest request, CancellationToken ct)
    {
        await enableToVoidHandler.HandleAsync(new SetEnableToVoidCommand(id, request.EnableToVoid, CurrentUser), ct);
        return NoContent();
    }
}

public record CreateRegistrasiPenjualanRequest(
    string       NoPenjualan,
    DateOnly     SaleDate,
    Guid         KaryawanId,
    Guid         KiosId,
    Guid?        MediatorId,
    string       MetodePenjualan,
    string       TipePenjualan,
    string       NoMesin,
    string       NoRangka,
    string       NamaCustomer,
    string       Address,
    string       Phone,
    string?      Phone1,
    string?      Phone2,
    decimal      OffRoad,
    decimal      Bbn,
    decimal      Discount,
    decimal      Total,
    decimal      AmbilUang,
    decimal      Dp,
    decimal      Angsuran,
    Guid?        DaftarHargaLeasingId,
    string?      TenorCode,
    string       SerahTerimaKendaraanId,
    string?      TandaTerimaSementaraId,
    List<string> Kelengkapan);

public record ApproveRegistrasiPenjualanRequest(decimal ApprovedDiscount);
public record SetEnableToVoidRequest(bool EnableToVoid);
