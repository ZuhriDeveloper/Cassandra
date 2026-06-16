using Cassandra.Application.Contracts.GrupTenor;
using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTenor;

namespace Cassandra.Application.Commands.GrupTenor.UpdateGrupTenor;

public class UpdateGrupTenorCommandHandler(
    IGrupTenorRepository repository,
    IGrupTenorQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateGrupTenorCommand command, CancellationToken ct = default)
    {
        var gt = await repository.GetByIdAsync(GrupTenorId.From(command.Id), ct)
            ?? throw new DomainException($"Grup tenor dengan id '{command.Id}' tidak ditemukan.");

        if (await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama grup tenor '{command.Name}' sudah ada.");

        gt.Update(command.Name, command.UpdatedBy);
        await repository.SaveAsync(gt, ct);
    }
}
