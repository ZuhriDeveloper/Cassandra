using Cassandra.Application.Contracts.Stnk;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;

namespace Cassandra.Application.Commands.Stnk.HandoverStnk;

public class HandoverStnkCommandHandler(IStnkRepository repository)
{
    public async Task HandleAsync(HandoverStnkCommand command, CancellationToken ct = default)
    {
        var stnk = await repository.GetByIdAsync(StnkId.From(command.StnkId), ct);
        if (stnk is null)
            throw new DomainException($"STNK dengan ID '{command.StnkId}' tidak ditemukan.");

        stnk.HandOver(command.HandoverDate, command.StnkReceiver, command.UpdatedBy);
        await repository.SaveAsync(stnk, ct);
    }
}
