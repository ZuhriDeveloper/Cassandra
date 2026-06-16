using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.DaftarHargaLeasing.CreateDaftarHargaLeasing;

public class CreateDaftarHargaLeasingCommandHandler(
    IDaftarHargaLeasingRepository repository,
    IDaftarHargaLeasingQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateDaftarHargaLeasingCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.ExistsAsync(command.Name.Trim(), command.GlobalLeasingId, command.GrupTenorId, ct))
            throw new DomainException($"Daftar harga leasing '{command.Name}' sudah ada untuk kombinasi leasing dan grup tenor ini.");

        var dhl = Domain.DaftarHargaLeasing.DaftarHargaLeasing.Create(
            command.Name, command.GlobalLeasingId, command.GrupTenorId, command.CreatedBy, dealerId);
        await repository.SaveAsync(dhl, ct);
        return dhl.Id.Value;
    }
}
