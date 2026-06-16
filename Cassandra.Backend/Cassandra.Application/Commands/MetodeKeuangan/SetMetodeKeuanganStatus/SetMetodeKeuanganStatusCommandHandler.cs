using Cassandra.Application.Contracts.MetodeKeuangan;
using Cassandra.Domain.Common;
using Cassandra.Domain.MetodeKeuangan;

namespace Cassandra.Application.Commands.MetodeKeuangan.SetMetodeKeuanganStatus;

public class SetMetodeKeuanganStatusCommandHandler(IMetodeKeuanganRepository repository)
{
    public async Task HandleAsync(SetMetodeKeuanganStatusCommand command, CancellationToken ct = default)
    {
        var mk = await repository.GetByIdAsync(MetodeKeuanganId.From(command.Id), ct)
            ?? throw new DomainException($"Metode keuangan dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            mk.Activate(command.ActionBy);
        else
            mk.Deactivate(command.ActionBy);

        await repository.SaveAsync(mk, ct);
    }
}
