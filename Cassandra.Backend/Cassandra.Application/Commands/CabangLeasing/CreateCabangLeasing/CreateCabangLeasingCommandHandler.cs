using Cassandra.Application.Contracts.CabangLeasing;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.CabangLeasing.CreateCabangLeasing;

public class CreateCabangLeasingCommandHandler(
    ICabangLeasingRepository repository,
    ICabangLeasingQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateCabangLeasingCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var code = command.Code.Trim().ToUpper();

        if (await queryRepository.CodeExistsAsync(code, ct))
            throw new DomainException($"Kode cabang leasing '{code}' sudah ada.");

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama cabang leasing '{command.Name}' sudah ada.");

        var cl = Domain.CabangLeasing.CabangLeasing.Create(
            command.Code, command.Name, command.Phone, command.Fax,
            command.Contact, command.GlobalLeasingId, command.CreatedBy, dealerId);
        await repository.SaveAsync(cl, ct);
        return cl.Id.Value;
    }
}
