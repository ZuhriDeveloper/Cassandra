using Cassandra.Application.Contracts.Bpkb;
using Cassandra.Domain.Bpkb;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Bpkb.ReceiveBpkb;

public class ReceiveBpkbCommandHandler(IBpkbRepository repository)
{
    public async Task HandleAsync(ReceiveBpkbCommand command, CancellationToken ct = default)
    {
        var bpkb = await repository.GetByIdAsync(BpkbId.From(command.BpkbId), ct);
        if (bpkb is null)
            throw new DomainException($"BPKB dengan ID '{command.BpkbId}' tidak ditemukan.");

        bpkb.Receive(command.BpkbNumber, command.BookNumber, command.ReceiveDate, command.UpdatedBy);
        await repository.SaveAsync(bpkb, ct);
    }
}
