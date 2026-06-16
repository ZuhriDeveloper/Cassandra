using Cassandra.Application.Contracts.DiscountCash;
using Cassandra.Domain.Common;
using Cassandra.Domain.DiscountCash;

namespace Cassandra.Application.Commands.DiscountCash.SetDiscountCashStatus;

public class SetDiscountCashStatusCommandHandler(IDiscountCashRepository repository)
{
    public async Task HandleAsync(SetDiscountCashStatusCommand command, CancellationToken ct = default)
    {
        var dc = await repository.GetByIdAsync(DiscountCashId.From(command.Id), ct)
            ?? throw new DomainException($"Discount cash dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            dc.Activate(command.ActionBy);
        else
            dc.Deactivate(command.ActionBy);

        await repository.SaveAsync(dc, ct);
    }
}
