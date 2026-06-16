using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Tenor;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Tenor.CreateTenor;

public class CreateTenorCommandHandler(
    ITenorRepository repository,
    ITenorQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateTenorCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var code = command.Code.Trim().ToUpper();

        if (await queryRepository.CodeExistsAsync(code, ct))
            throw new DomainException($"Kode tenor '{code}' sudah ada.");

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama tenor '{command.Name}' sudah ada.");

        var tenor = Domain.Tenor.Tenor.Create(command.Code, command.Name, command.Months, command.GrupTenorId, command.CreatedBy, dealerId);
        await repository.SaveAsync(tenor, ct);
        return tenor.Id.Value;
    }
}
