using Cassandra.Application.Contracts.GlobalLeasing;
using Cassandra.Domain.Common;
using Cassandra.Domain.GlobalLeasing;

namespace Cassandra.Application.Commands.GlobalLeasing.UpdateGlobalLeasing;

public class UpdateGlobalLeasingCommandHandler(
    IGlobalLeasingRepository repository,
    IGlobalLeasingQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateGlobalLeasingCommand command, CancellationToken ct = default)
    {
        var gl = await repository.GetByIdAsync(GlobalLeasingId.From(command.Id), ct)
            ?? throw new DomainException($"Global leasing dengan id '{command.Id}' tidak ditemukan.");

        if (await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama global leasing '{command.Name}' sudah ada.");

        gl.Update(command.Name, command.Phone, command.Fax, command.Contact, command.Address, command.UpdatedBy);
        await repository.SaveAsync(gl, ct);
    }
}
