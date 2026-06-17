using Cassandra.Application.Contracts.ApTransaction;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.ApTransaction;
using Domain = Cassandra.Domain;

namespace Cassandra.Application.Commands.ApTransaction.CreateApTransaction;

public class CreateApTransactionCommandHandler(
    IApTransactionRepository repository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateApTransactionCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        var ap = Domain.ApTransaction.ApTransaction.Create(
            ApTransactionId.New(),
            command.TransactionType,
            command.StnkId,
            command.NoRangka,
            command.TotalAmount,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(ap, ct);
        return ap.Id.Value;
    }
}
