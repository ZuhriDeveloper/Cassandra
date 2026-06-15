using Cassandra.Application.Contracts.Kelengkapan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Kelengkapan;

namespace Cassandra.Application.Commands.Kelengkapan.SetKelengkapanStatus;

public class SetKelengkapanStatusCommandHandler(IKelengkapanRepository repository)
{
    public async Task HandleAsync(SetKelengkapanStatusCommand command, CancellationToken ct = default)
    {
        var kelengkapan = await repository.GetByIdAsync(KelengkapanId.From(command.Id), ct)
            ?? throw new DomainException($"Kelengkapan dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            kelengkapan.Activate(command.UpdatedBy);
        else
            kelengkapan.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(kelengkapan, ct);
    }
}
