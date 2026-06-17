using Cassandra.Application.Contracts.So;
using Cassandra.Application.Contracts.SoPenerimaan;
using Cassandra.Domain.Common;
using Cassandra.Domain.So;

namespace Cassandra.Application.Commands.So.DeleteSo;

public class DeleteSoCommandHandler(
    ISoRepository repository,
    ISoPenerimaanQueryRepository penerimaanQueryRepository)
{
    public async Task HandleAsync(DeleteSoCommand command, CancellationToken ct = default)
    {
        var so = await repository.GetByIdAsync(SoId.From(command.SoId), ct)
            ?? throw new DomainException($"SO dengan ID '{command.SoId}' tidak ditemukan.");

        if (await penerimaanQueryRepository.HasPenerimaanForSoAsync(command.SoId, ct))
            throw new DomainException("SO tidak dapat dihapus karena sudah ada penerimaan barang.");

        so.Delete(command.DeletedBy);
        await repository.SaveAsync(so, ct);
    }
}
