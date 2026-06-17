using Cassandra.Application.Contracts.Stnk;
using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;

namespace Cassandra.Application.Commands.Stnk.ProcessStnk;

public class ProcessStnkCommandHandler(IStnkRepository repository)
{
    public async Task HandleAsync(ProcessStnkCommand command, CancellationToken ct = default)
    {
        var stnk = await repository.GetByIdAsync(StnkId.From(command.StnkId), ct);
        if (stnk is null)
            throw new DomainException($"STNK dengan ID '{command.StnkId}' tidak ditemukan.");

        stnk.Process(command.ProcessDate, command.BiroId, command.InvoiceNumber, command.UpdatedBy);
        await repository.SaveAsync(stnk, ct);
    }
}
