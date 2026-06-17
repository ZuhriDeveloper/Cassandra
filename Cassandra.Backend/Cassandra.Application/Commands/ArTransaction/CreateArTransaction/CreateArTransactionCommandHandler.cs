using Cassandra.Application.Contracts.ArTransaction;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.ArTransaction;
using Domain = Cassandra.Domain;

namespace Cassandra.Application.Commands.ArTransaction.CreateArTransaction;

public class CreateArTransactionCommandHandler(
    IArTransactionRepository repository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateArTransactionCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        var ar = Domain.ArTransaction.ArTransaction.Create(
            ArTransactionId.New(),
            command.TransactionType,
            command.ReferenceId,
            command.ReferenceNumber,
            command.TotalAmount,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(ar, ct);
        return ar.Id.Value;
    }
}
