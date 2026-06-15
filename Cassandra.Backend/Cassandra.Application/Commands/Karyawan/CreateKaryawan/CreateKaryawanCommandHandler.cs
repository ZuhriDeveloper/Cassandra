using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;

namespace Cassandra.Application.Commands.Karyawan.CreateKaryawan;

public class CreateKaryawanCommandHandler(
    IKaryawanRepository      repository,
    IKaryawanQueryRepository queryRepository,
    ICurrentDealer           currentDealer)
{
    public async Task<Guid> HandleAsync(CreateKaryawanCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.EmailExistsAsync(command.Email.Trim(), ct))
            throw new DomainException($"Email '{command.Email}' sudah digunakan oleh karyawan lain.");

        var karyawan = Domain.Karyawan.Karyawan.Create(
            command.Name,
            command.Email,
            command.KtpNumber,
            command.Gender,
            command.HireDate,
            command.Phone,
            command.PhoneAlt,
            command.Address,
            command.SalesLimit,
            command.JabatanId,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(karyawan, ct);
        return karyawan.Id.Value;
    }
}
