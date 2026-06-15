using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;

namespace Cassandra.Application.Commands.Karyawan.UpdateKaryawan;

public class UpdateKaryawanCommandHandler(
    IKaryawanRepository      repository,
    IKaryawanQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateKaryawanCommand command, CancellationToken ct = default)
    {
        var karyawan = await repository.GetByIdAsync(KaryawanId.From(command.Id), ct)
            ?? throw new DomainException($"Karyawan dengan id '{command.Id}' tidak ditemukan.");

        var emailTrimmed = command.Email.Trim();
        if (!string.Equals(karyawan.Email, emailTrimmed, StringComparison.OrdinalIgnoreCase) &&
            await queryRepository.EmailExistsExcludingAsync(emailTrimmed, command.Id, ct))
            throw new DomainException($"Email '{command.Email}' sudah digunakan oleh karyawan lain.");

        karyawan.Update(command.Name, command.Email, command.Phone, command.PhoneAlt, command.Address, command.JabatanId, command.UpdatedBy);
        await repository.SaveAsync(karyawan, ct);
    }
}
