using Cassandra.Application.Contracts.CabangLeasing;
using Cassandra.Domain.CabangLeasing;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.CabangLeasing.SetCabangLeasingStatus;

public class SetCabangLeasingStatusCommandHandler(ICabangLeasingRepository repository)
{
    public async Task HandleAsync(SetCabangLeasingStatusCommand command, CancellationToken ct = default)
    {
        var cl = await repository.GetByIdAsync(CabangLeasingId.From(command.Id), ct)
            ?? throw new DomainException($"Cabang leasing dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            cl.Activate(command.ActionBy);
        else
            cl.Deactivate(command.ActionBy);

        await repository.SaveAsync(cl, ct);
    }
}
