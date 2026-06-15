using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Kios;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Kios.CreateKios;

public class CreateKiosCommandHandler(
    IKiosRepository      repository,
    IKiosQueryRepository queryRepository,
    ICurrentDealer       currentDealer)
{
    public async Task<Guid> HandleAsync(CreateKiosCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.CodeExistsAsync(command.Code.Trim().ToUpper(), ct))
            throw new DomainException($"Kode kios '{command.Code}' sudah digunakan.");

        var kios = Domain.Kios.Kios.Create(
            command.Code,
            command.Name,
            command.Phone,
            command.Fax,
            command.Address,
            command.PicKaryawanId,
            command.Limit,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(kios, ct);
        return kios.Id.Value;
    }
}
