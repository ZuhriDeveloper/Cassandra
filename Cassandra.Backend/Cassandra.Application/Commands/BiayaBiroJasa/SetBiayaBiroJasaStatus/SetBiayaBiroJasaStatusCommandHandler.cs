using Cassandra.Application.Contracts.BiayaBiroJasa;
using Cassandra.Domain.BiayaBiroJasa;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.BiayaBiroJasa.SetBiayaBiroJasaStatus;

public class SetBiayaBiroJasaStatusCommandHandler(IBiayaBiroJasaRepository repository)
{
    public async Task HandleAsync(SetBiayaBiroJasaStatusCommand command, CancellationToken ct = default)
    {
        var bbj = await repository.GetByIdAsync(BiayaBiroJasaId.From(command.Id), ct)
            ?? throw new DomainException("Biaya biro jasa tidak ditemukan.");

        if (command.IsActive)
            bbj.Activate(command.UpdatedBy);
        else
            bbj.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(bbj, ct);
    }
}
