using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.GrupTenor;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.GrupTenor.CreateGrupTenor;

public class CreateGrupTenorCommandHandler(
    IGrupTenorRepository repository,
    IGrupTenorQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateGrupTenorCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var code = command.Code.Trim().ToUpper();

        if (await queryRepository.CodeExistsAsync(code, ct))
            throw new DomainException($"Kode grup tenor '{code}' sudah ada.");

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama grup tenor '{command.Name}' sudah ada.");

        var gt = Domain.GrupTenor.GrupTenor.Create(command.Code, command.Name, command.CreatedBy, dealerId);
        await repository.SaveAsync(gt, ct);
        return gt.Id.Value;
    }
}
