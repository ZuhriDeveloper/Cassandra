using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Jabatan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Jabatan;

namespace Cassandra.Application.Commands.Jabatan.CreateJabatan;

public class CreateJabatanCommandHandler(
    IJabatanRepository repository,
    IJabatanQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateJabatanCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Jabatan '{command.Name}' sudah ada.");

        var jabatan = Domain.Jabatan.Jabatan.Create(command.Name, command.Description, command.CreatedBy, dealerId);
        await repository.SaveAsync(jabatan, ct);
        return jabatan.Id.Value;
    }
}
