using Cassandra.Application.Contracts.Jabatan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Jabatan;

namespace Cassandra.Application.Commands.Jabatan.SetJabatanStatus;

public class SetJabatanStatusCommandHandler(IJabatanRepository repository)
{
    public async Task HandleAsync(SetJabatanStatusCommand command, CancellationToken ct = default)
    {
        var jabatan = await repository.GetByIdAsync(JabatanId.From(command.Id), ct)
            ?? throw new DomainException($"Jabatan dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            jabatan.Activate(command.ActionBy);
        else
            jabatan.Deactivate(command.ActionBy);

        await repository.SaveAsync(jabatan, ct);
    }
}
