using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Domain.Common;
using Cassandra.Domain.DaftarHargaLeasing;

namespace Cassandra.Application.Commands.DaftarHargaLeasing.UpdateDaftarHargaLeasing;

public class UpdateDaftarHargaLeasingCommandHandler(
    IDaftarHargaLeasingRepository repository,
    IDaftarHargaLeasingQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateDaftarHargaLeasingCommand command, CancellationToken ct = default)
    {
        var dhl = await repository.GetByIdAsync(DaftarHargaLeasingId.From(command.Id), ct)
            ?? throw new DomainException($"Daftar harga leasing dengan id '{command.Id}' tidak ditemukan.");

        if (await queryRepository.ExistsExcludingAsync(command.Name.Trim(), command.GlobalLeasingId, command.GrupTenorId, command.Id, ct))
            throw new DomainException($"Daftar harga leasing '{command.Name}' sudah ada untuk kombinasi leasing dan grup tenor ini.");

        dhl.Update(command.Name, command.GlobalLeasingId, command.GrupTenorId, command.UpdatedBy);
        await repository.SaveAsync(dhl, ct);
    }
}
