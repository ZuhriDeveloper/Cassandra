using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;

namespace Cassandra.Application.Commands.Karyawan.SetKaryawanLimit;

public class SetKaryawanLimitCommandHandler(IKaryawanRepository repository)
{
    public async Task HandleAsync(SetKaryawanLimitCommand command, CancellationToken ct = default)
    {
        var karyawan = await repository.GetByIdAsync(KaryawanId.From(command.Id), ct)
            ?? throw new DomainException($"Karyawan dengan id '{command.Id}' tidak ditemukan.");

        karyawan.SetLimit(command.SalesLimit, command.SetBy);
        await repository.SaveAsync(karyawan, ct);
    }
}
