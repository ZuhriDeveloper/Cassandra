using Cassandra.Application.Contracts.GlobalLeasing;
using Cassandra.Domain.Common;
using Cassandra.Domain.GlobalLeasing;

namespace Cassandra.Application.Commands.GlobalLeasing.SetGlobalLeasingStatus;

public class SetGlobalLeasingStatusCommandHandler(IGlobalLeasingRepository repository)
{
    public async Task HandleAsync(SetGlobalLeasingStatusCommand command, CancellationToken ct = default)
    {
        var gl = await repository.GetByIdAsync(GlobalLeasingId.From(command.Id), ct)
            ?? throw new DomainException($"Global leasing dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            gl.Activate(command.ActionBy);
        else
            gl.Deactivate(command.ActionBy);

        await repository.SaveAsync(gl, ct);
    }
}
