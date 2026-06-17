using Cassandra.Application.Contracts.Bpkb;
using Cassandra.Domain.Bpkb;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Bpkb.HandoverBpkb;

public class HandoverBpkbCommandHandler(IBpkbRepository repository)
{
    public async Task HandleAsync(HandoverBpkbCommand command, CancellationToken ct = default)
    {
        var bpkb = await repository.GetByIdAsync(BpkbId.From(command.BpkbId), ct);
        if (bpkb is null)
            throw new DomainException($"BPKB dengan ID '{command.BpkbId}' tidak ditemukan.");

        bpkb.HandOver(command.HandoverDate, command.Receiver, command.UpdatedBy);
        await repository.SaveAsync(bpkb, ct);
    }
}
