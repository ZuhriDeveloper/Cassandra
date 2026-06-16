using Cassandra.Application.Contracts.Biro;
using Cassandra.Domain.Biro;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Biro.UpdateBiro;

public class UpdateBiroCommandHandler(
    IBiroRepository repository,
    IBiroQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateBiroCommand command, CancellationToken ct = default)
    {
        var biro = await repository.GetByIdAsync(BiroId.From(command.Id), ct)
            ?? throw new DomainException("Biro tidak ditemukan.");

        if (await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama biro '{command.Name.Trim()}' sudah ada.");

        biro.Update(command.Name, command.Phone, command.Fax, command.Pic, command.Address, command.PphRate, command.UpdatedBy);
        await repository.SaveAsync(biro, ct);
    }
}
