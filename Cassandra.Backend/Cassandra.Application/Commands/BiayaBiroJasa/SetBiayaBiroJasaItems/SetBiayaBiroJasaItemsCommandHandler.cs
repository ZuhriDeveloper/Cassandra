using Cassandra.Application.Contracts.BiayaBiroJasa;
using Cassandra.Domain.BiayaBiroJasa;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.BiayaBiroJasa.SetBiayaBiroJasaItems;

public class SetBiayaBiroJasaItemsCommandHandler(IBiayaBiroJasaRepository repository)
{
    public async Task HandleAsync(SetBiayaBiroJasaItemsCommand command, CancellationToken ct = default)
    {
        var bbj = await repository.GetByIdAsync(BiayaBiroJasaId.From(command.Id), ct)
            ?? throw new DomainException("Biaya biro jasa tidak ditemukan.");

        bbj.SetItems(command.Items, command.UpdatedBy);
        await repository.SaveAsync(bbj, ct);
    }
}
