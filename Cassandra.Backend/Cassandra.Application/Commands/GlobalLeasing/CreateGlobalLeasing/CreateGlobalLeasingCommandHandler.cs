using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.GlobalLeasing;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.GlobalLeasing.CreateGlobalLeasing;

public class CreateGlobalLeasingCommandHandler(
    IGlobalLeasingRepository repository,
    IGlobalLeasingQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateGlobalLeasingCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var code = command.Code.Trim().ToUpper();

        if (await queryRepository.CodeExistsAsync(code, ct))
            throw new DomainException($"Kode global leasing '{code}' sudah ada.");

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama global leasing '{command.Name}' sudah ada.");

        var gl = Domain.GlobalLeasing.GlobalLeasing.Create(
            command.Code, command.Name, command.Phone, command.Fax,
            command.Contact, command.Address, command.CreatedBy, dealerId);
        await repository.SaveAsync(gl, ct);
        return gl.Id.Value;
    }
}
