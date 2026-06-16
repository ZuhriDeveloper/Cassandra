using Cassandra.Application.Contracts.MetodeKeuangan;
using Cassandra.Domain.Common;
using Cassandra.Domain.MetodeKeuangan;

namespace Cassandra.Application.Commands.MetodeKeuangan.UpdateMetodeKeuangan;

public class UpdateMetodeKeuanganCommandHandler(
    IMetodeKeuanganRepository repository,
    IMetodeKeuanganQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateMetodeKeuanganCommand command, CancellationToken ct = default)
    {
        var mk = await repository.GetByIdAsync(MetodeKeuanganId.From(command.Id), ct)
            ?? throw new DomainException($"Metode keuangan dengan id '{command.Id}' tidak ditemukan.");

        if (await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama metode keuangan '{command.Name}' sudah ada.");

        mk.Update(command.Name, command.UpdatedBy);
        await repository.SaveAsync(mk, ct);
    }
}
