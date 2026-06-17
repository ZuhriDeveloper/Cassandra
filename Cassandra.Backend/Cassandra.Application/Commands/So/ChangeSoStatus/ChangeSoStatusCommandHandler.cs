using Cassandra.Application.Contracts.So;
using Cassandra.Domain.Common;
using Cassandra.Domain.So;

namespace Cassandra.Application.Commands.So.ChangeSoStatus;

public class ChangeSoStatusCommandHandler(ISoRepository repository)
{
    public async Task HandleAsync(ChangeSoStatusCommand command, CancellationToken ct = default)
    {
        var so = await repository.GetByIdAsync(SoId.From(command.SoId), ct)
            ?? throw new DomainException($"SO dengan ID '{command.SoId}' tidak ditemukan.");

        so.ChangeStatus(command.Status, command.UpdatedBy);
        await repository.SaveAsync(so, ct);
    }
}
