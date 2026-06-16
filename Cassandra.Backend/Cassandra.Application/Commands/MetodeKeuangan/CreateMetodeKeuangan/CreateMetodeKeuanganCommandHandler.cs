using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.MetodeKeuangan;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.MetodeKeuangan.CreateMetodeKeuangan;

public class CreateMetodeKeuanganCommandHandler(
    IMetodeKeuanganRepository repository,
    IMetodeKeuanganQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateMetodeKeuanganCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var code = command.Code.Trim().ToUpper();

        if (await queryRepository.CodeExistsAsync(code, ct))
            throw new DomainException($"Kode metode keuangan '{code}' sudah ada.");

        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama metode keuangan '{command.Name}' sudah ada.");

        var mk = Domain.MetodeKeuangan.MetodeKeuangan.Create(command.Code, command.Name, command.CreatedBy, dealerId);
        await repository.SaveAsync(mk, ct);
        return mk.Id.Value;
    }
}
