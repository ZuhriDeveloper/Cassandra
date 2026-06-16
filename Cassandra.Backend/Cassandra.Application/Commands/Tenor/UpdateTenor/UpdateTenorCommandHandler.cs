using Cassandra.Application.Contracts.Tenor;
using Cassandra.Domain.Common;
using Cassandra.Domain.Tenor;

namespace Cassandra.Application.Commands.Tenor.UpdateTenor;

public class UpdateTenorCommandHandler(
    ITenorRepository repository,
    ITenorQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateTenorCommand command, CancellationToken ct = default)
    {
        var tenor = await repository.GetByIdAsync(TenorId.From(command.Id), ct)
            ?? throw new DomainException($"Tenor dengan id '{command.Id}' tidak ditemukan.");

        if (await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama tenor '{command.Name}' sudah ada.");

        tenor.Update(command.Name, command.Months, command.GrupTenorId, command.UpdatedBy);
        await repository.SaveAsync(tenor, ct);
    }
}
