using Cassandra.Application.Contracts.DaftarHargaLeasing;
using Cassandra.Domain.Common;
using Cassandra.Domain.DaftarHargaLeasing;

namespace Cassandra.Application.Commands.DaftarHargaLeasing.SetDaftarHargaLeasingItems;

public class SetDaftarHargaLeasingItemsCommandHandler(IDaftarHargaLeasingRepository repository)
{
    public async Task HandleAsync(SetDaftarHargaLeasingItemsCommand command, CancellationToken ct = default)
    {
        var dhl = await repository.GetByIdAsync(DaftarHargaLeasingId.From(command.Id), ct)
            ?? throw new DomainException($"Daftar harga leasing dengan id '{command.Id}' tidak ditemukan.");

        var items = command.Items
            .Select(i => new DaftarHargaLeasingItem(i.GrupTipeMotorId, i.Subsidi, i.Incentive, i.LainLain))
            .ToList()
            .AsReadOnly();

        dhl.SetItems(items, command.UpdatedBy);
        await repository.SaveAsync(dhl, ct);
    }
}
