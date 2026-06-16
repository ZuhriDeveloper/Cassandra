using Cassandra.Application.Contracts.Biro;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Biro.CreateBiro;

public class CreateBiroCommandHandler(
    IBiroRepository repository,
    IBiroQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateBiroCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var code = command.Code.Trim().ToUpper();

        if (await queryRepository.CodeExistsAsync(code, ct))
            throw new DomainException($"Kode biro '{code}' sudah ada.");
        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama biro '{command.Name.Trim()}' sudah ada.");

        var biro = Domain.Biro.Biro.Create(
            command.Code,
            command.Name,
            command.Phone,
            command.Fax,
            command.Pic,
            command.Address,
            command.PphRate,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(biro, ct);
        return biro.Id.Value;
    }
}
