using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;

namespace Cassandra.Application.Commands.Karyawan.RecordKaryawanResign;

public class RecordKaryawanResignCommandHandler(IKaryawanRepository repository)
{
    public async Task HandleAsync(RecordKaryawanResignCommand command, CancellationToken ct = default)
    {
        var karyawan = await repository.GetByIdAsync(KaryawanId.From(command.Id), ct)
            ?? throw new DomainException($"Karyawan dengan id '{command.Id}' tidak ditemukan.");

        karyawan.RecordResign(command.ResignDate, command.RecordedBy);
        await repository.SaveAsync(karyawan, ct);
    }
}
