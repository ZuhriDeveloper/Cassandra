using Cassandra.Application.Contracts.Kelengkapan;
using Cassandra.Domain.Common;
using Cassandra.Domain.Kelengkapan;

namespace Cassandra.Application.Commands.Kelengkapan.UpdateKelengkapan;

public class UpdateKelengkapanCommandHandler(
    IKelengkapanRepository repository,
    IKelengkapanQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateKelengkapanCommand command, CancellationToken ct = default)
    {
        var kelengkapan = await repository.GetByIdAsync(KelengkapanId.From(command.Id), ct)
            ?? throw new DomainException($"Kelengkapan dengan id '{command.Id}' tidak ditemukan.");

        if (!string.Equals(kelengkapan.Name, command.Name.Trim(), StringComparison.OrdinalIgnoreCase)
            && await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Kelengkapan '{command.Name}' sudah ada.");

        kelengkapan.Update(command.Name, command.UpdatedBy);
        await repository.SaveAsync(kelengkapan, ct);
    }
}
