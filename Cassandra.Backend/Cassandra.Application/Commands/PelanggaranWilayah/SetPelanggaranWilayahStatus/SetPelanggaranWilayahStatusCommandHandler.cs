using Cassandra.Application.Contracts.PelanggaranWilayah;
using Cassandra.Domain.Common;
using Cassandra.Domain.PelanggaranWilayah;

namespace Cassandra.Application.Commands.PelanggaranWilayah.SetPelanggaranWilayahStatus;

public class SetPelanggaranWilayahStatusCommandHandler(IPelanggaranWilayahRepository repository)
{
    public async Task HandleAsync(SetPelanggaranWilayahStatusCommand command, CancellationToken ct = default)
    {
        var pw = await repository.GetByIdAsync(PelanggaranWilayahId.From(command.Id), ct)
            ?? throw new DomainException("Pelanggaran wilayah tidak ditemukan.");

        if (command.IsActive)
            pw.Activate(command.UpdatedBy);
        else
            pw.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(pw, ct);
    }
}
