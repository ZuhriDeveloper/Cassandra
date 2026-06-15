using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;

namespace Cassandra.Application.Commands.Karyawan.SetKaryawanStatus;

public class SetKaryawanStatusCommandHandler(IKaryawanRepository repository)
{
    public async Task HandleAsync(SetKaryawanStatusCommand command, CancellationToken ct = default)
    {
        var karyawan = await repository.GetByIdAsync(KaryawanId.From(command.Id), ct)
            ?? throw new DomainException($"Karyawan dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            karyawan.Activate(command.ActionBy);
        else
            karyawan.Deactivate(command.ActionBy);

        await repository.SaveAsync(karyawan, ct);
    }
}
