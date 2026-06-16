using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Samsat;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Samsat.CreateSamsat;

public class CreateSamsatCommandHandler(
    ISamsatRepository repository,
    ISamsatQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateSamsatCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama samsat '{command.Name.Trim()}' sudah ada.");

        var samsat = Domain.Samsat.Samsat.Create(command.Name, command.CreatedBy, dealerId);
        await repository.SaveAsync(samsat, ct);
        return samsat.Id.Value;
    }
}
