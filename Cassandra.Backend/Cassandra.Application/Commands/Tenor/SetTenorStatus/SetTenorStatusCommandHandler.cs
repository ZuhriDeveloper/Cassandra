using Cassandra.Application.Contracts.Tenor;
using Cassandra.Domain.Common;
using Cassandra.Domain.Tenor;

namespace Cassandra.Application.Commands.Tenor.SetTenorStatus;

public class SetTenorStatusCommandHandler(ITenorRepository repository)
{
    public async Task HandleAsync(SetTenorStatusCommand command, CancellationToken ct = default)
    {
        var tenor = await repository.GetByIdAsync(TenorId.From(command.Id), ct)
            ?? throw new DomainException($"Tenor dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            tenor.Activate(command.ActionBy);
        else
            tenor.Deactivate(command.ActionBy);

        await repository.SaveAsync(tenor, ct);
    }
}
