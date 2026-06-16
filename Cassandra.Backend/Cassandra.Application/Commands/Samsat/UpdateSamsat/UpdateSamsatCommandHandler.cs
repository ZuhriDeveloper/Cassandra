using Cassandra.Application.Contracts.Samsat;
using Cassandra.Domain.Common;
using Cassandra.Domain.Samsat;

namespace Cassandra.Application.Commands.Samsat.UpdateSamsat;

public class UpdateSamsatCommandHandler(
    ISamsatRepository repository,
    ISamsatQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateSamsatCommand command, CancellationToken ct = default)
    {
        var samsat = await repository.GetByIdAsync(SamsatId.From(command.Id), ct)
            ?? throw new DomainException("Samsat tidak ditemukan.");

        if (await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama samsat '{command.Name.Trim()}' sudah ada.");

        samsat.Update(command.Name, command.UpdatedBy);
        await repository.SaveAsync(samsat, ct);
    }
}
