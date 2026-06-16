using Cassandra.Application.Contracts.GrupTenor;
using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTenor;

namespace Cassandra.Application.Commands.GrupTenor.SetGrupTenorStatus;

public class SetGrupTenorStatusCommandHandler(IGrupTenorRepository repository)
{
    public async Task HandleAsync(SetGrupTenorStatusCommand command, CancellationToken ct = default)
    {
        var gt = await repository.GetByIdAsync(GrupTenorId.From(command.Id), ct)
            ?? throw new DomainException($"Grup tenor dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            gt.Activate(command.ActionBy);
        else
            gt.Deactivate(command.ActionBy);

        await repository.SaveAsync(gt, ct);
    }
}
