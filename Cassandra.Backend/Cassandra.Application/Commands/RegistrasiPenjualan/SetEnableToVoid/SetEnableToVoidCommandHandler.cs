using Cassandra.Application.Contracts.RegistrasiPenjualan;
using Cassandra.Domain.Common;
using Cassandra.Domain.RegistrasiPenjualan;

namespace Cassandra.Application.Commands.RegistrasiPenjualan.SetEnableToVoid;

public class SetEnableToVoidCommandHandler(IRegistrasiPenjualanRepository registrasiRepo)
{
    public async Task HandleAsync(SetEnableToVoidCommand command, CancellationToken ct = default)
    {
        var reg = await registrasiRepo.GetByIdAsync(RegistrasiPenjualanId.From(command.Id), ct)
            ?? throw new DomainException("Registrasi penjualan tidak ditemukan.");

        reg.SetEnableToVoid(command.EnableToVoid, command.UpdatedBy);
        await registrasiRepo.SaveAsync(reg, ct);
    }
}
