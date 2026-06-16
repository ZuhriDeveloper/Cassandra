using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Domain.Common;
using Cassandra.Domain.DaftarHargaLeasing;

namespace Cassandra.Application.Commands.DaftarHargaLeasing.SetDaftarHargaLeasingStatus;

public class SetDaftarHargaLeasingStatusCommandHandler(IDaftarHargaLeasingRepository repository)
{
    public async Task HandleAsync(SetDaftarHargaLeasingStatusCommand command, CancellationToken ct = default)
    {
        var dhl = await repository.GetByIdAsync(DaftarHargaLeasingId.From(command.Id), ct)
            ?? throw new DomainException($"Daftar harga leasing dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            dhl.Activate(command.ActionBy);
        else
            dhl.Deactivate(command.ActionBy);

        await repository.SaveAsync(dhl, ct);
    }
}
