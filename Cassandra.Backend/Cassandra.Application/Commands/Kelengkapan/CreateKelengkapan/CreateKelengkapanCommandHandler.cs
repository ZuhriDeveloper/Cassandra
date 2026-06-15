using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Kelengkapan;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Kelengkapan.CreateKelengkapan;

public class CreateKelengkapanCommandHandler(
    IKelengkapanRepository repository,
    IKelengkapanQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateKelengkapanCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Kelengkapan '{command.Name}' sudah ada.");

        var kelengkapan = Domain.Kelengkapan.Kelengkapan.Create(command.Name, command.CreatedBy, dealerId);
        await repository.SaveAsync(kelengkapan, ct);
        return kelengkapan.Id.Value;
    }
}
