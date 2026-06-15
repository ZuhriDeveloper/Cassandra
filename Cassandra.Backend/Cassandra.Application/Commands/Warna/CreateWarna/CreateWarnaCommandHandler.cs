using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Warna;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Warna.CreateWarna;

public class CreateWarnaCommandHandler(
    IWarnaRepository repository,
    IWarnaQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateWarnaCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.CodeExistsAsync(command.Code.Trim().ToUpper(), ct))
            throw new DomainException($"Kode warna '{command.Code}' sudah digunakan.");

        var warna = Domain.Warna.Warna.Create(command.Code, command.Name, command.CreatedBy, dealerId);
        await repository.SaveAsync(warna, ct);
        return warna.Id.Value;
    }
}
