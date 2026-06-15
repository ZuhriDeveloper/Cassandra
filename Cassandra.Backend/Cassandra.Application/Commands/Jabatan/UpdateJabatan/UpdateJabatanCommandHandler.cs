using Cassandra.Application.Contracts.Jabatan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Jabatan;

namespace Cassandra.Application.Commands.Jabatan.UpdateJabatan;

public class UpdateJabatanCommandHandler(
    IJabatanRepository repository,
    IJabatanQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateJabatanCommand command, CancellationToken ct = default)
    {
        var jabatan = await repository.GetByIdAsync(JabatanId.From(command.Id), ct)
            ?? throw new DomainException($"Jabatan dengan id '{command.Id}' tidak ditemukan.");

        var nameChanged = !string.Equals(jabatan.Name, command.Name.Trim(), StringComparison.OrdinalIgnoreCase);
        if (nameChanged && await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Jabatan '{command.Name}' sudah ada.");

        jabatan.Update(command.Name, command.Description, command.UpdatedBy);
        await repository.SaveAsync(jabatan, ct);
    }
}
