using Cassandra.Application.Contracts.Kios;
using Cassandra.Domain.Common;
using Cassandra.Domain.Kios;

namespace Cassandra.Application.Commands.Kios.SetKiosLimit;

public class SetKiosLimitCommandHandler(IKiosRepository repository)
{
    public async Task HandleAsync(SetKiosLimitCommand command, CancellationToken ct = default)
    {
        var kios = await repository.GetByIdAsync(KiosId.From(command.Id), ct)
            ?? throw new DomainException($"Kios dengan id '{command.Id}' tidak ditemukan.");

        kios.SetLimit(command.Limit, command.SetBy);
        await repository.SaveAsync(kios, ct);
    }
}
