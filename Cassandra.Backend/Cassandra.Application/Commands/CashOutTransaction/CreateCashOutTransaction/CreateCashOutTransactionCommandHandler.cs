using Cassandra.Application.Contracts.CashOutTransaction;
using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Finance;
using Cassandra.Domain.CashOutTransaction;
using Domain = Cassandra.Domain;

namespace Cassandra.Application.Commands.CashOutTransaction.CreateCashOutTransaction;

public class CreateCashOutTransactionCommandHandler(
    ICashOutTransactionRepository repository,
    IFinanceCounter financeCounter,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateCashOutTransactionCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var fInvoiceId = await financeCounter.GetNextFInvoiceIdAsync(ct);

        var cashOut = Domain.CashOutTransaction.CashOutTransaction.Create(
            CashOutTransactionId.New(),
            command.TransactionType,
            command.SoId,
            command.SoReturId,
            command.Amount,
            command.DfAmount,
            command.TotalHariDf,
            command.PaymentDate,
            command.PaymentMethod,
            fInvoiceId,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(cashOut, ct);
        return cashOut.Id.Value;
    }
}
