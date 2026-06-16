using Cassandra.Application.Contracts.PelanggaranWilayah;
using Cassandra.Domain.Common;
using Cassandra.Domain.PelanggaranWilayah;

namespace Cassandra.Application.Commands.PelanggaranWilayah.UpdatePelanggaranWilayah;

public class UpdatePelanggaranWilayahCommandHandler(IPelanggaranWilayahRepository repository)
{
    public async Task HandleAsync(UpdatePelanggaranWilayahCommand command, CancellationToken ct = default)
    {
        var pw = await repository.GetByIdAsync(PelanggaranWilayahId.From(command.Id), ct)
            ?? throw new DomainException("Pelanggaran wilayah tidak ditemukan.");

        pw.Update(command.ExtraFee, command.UpdatedBy);
        await repository.SaveAsync(pw, ct);
    }
}
