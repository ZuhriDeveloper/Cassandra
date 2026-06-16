using Cassandra.Application.Contracts.CabangLeasing;
using Cassandra.Domain.CabangLeasing;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.CabangLeasing.UpdateCabangLeasing;

public class UpdateCabangLeasingCommandHandler(
    ICabangLeasingRepository repository,
    ICabangLeasingQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateCabangLeasingCommand command, CancellationToken ct = default)
    {
        var cl = await repository.GetByIdAsync(CabangLeasingId.From(command.Id), ct)
            ?? throw new DomainException($"Cabang leasing dengan id '{command.Id}' tidak ditemukan.");

        if (await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama cabang leasing '{command.Name}' sudah ada.");

        cl.Update(command.Name, command.Phone, command.Fax, command.Contact, command.GlobalLeasingId, command.UpdatedBy);
        await repository.SaveAsync(cl, ct);
    }
}
