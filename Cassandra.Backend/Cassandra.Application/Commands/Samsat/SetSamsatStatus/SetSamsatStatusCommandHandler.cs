using Cassandra.Application.Contracts.Samsat;
using Cassandra.Domain.Common;
using Cassandra.Domain.Samsat;

namespace Cassandra.Application.Commands.Samsat.SetSamsatStatus;

public class SetSamsatStatusCommandHandler(ISamsatRepository repository)
{
    public async Task HandleAsync(SetSamsatStatusCommand command, CancellationToken ct = default)
    {
        var samsat = await repository.GetByIdAsync(SamsatId.From(command.Id), ct)
            ?? throw new DomainException("Samsat tidak ditemukan.");

        if (command.IsActive)
            samsat.Activate(command.UpdatedBy);
        else
            samsat.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(samsat, ct);
    }
}
