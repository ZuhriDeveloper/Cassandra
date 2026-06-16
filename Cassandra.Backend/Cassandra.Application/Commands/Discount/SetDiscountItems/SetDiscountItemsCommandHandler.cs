using Cassandra.Application.Contracts.Discount;
using Cassandra.Domain.Common;
using Cassandra.Domain.Discount;

namespace Cassandra.Application.Commands.Discount.SetDiscountItems;

public class SetDiscountItemsCommandHandler(IDiscountRepository repository)
{
    public async Task HandleAsync(SetDiscountItemsCommand command, CancellationToken ct = default)
    {
        var discount = await repository.GetByIdAsync(DiscountId.From(command.Id), ct)
            ?? throw new DomainException($"Discount dengan id '{command.Id}' tidak ditemukan.");

        var items = command.Items
            .Select(i => new DiscountLineItem(i.GrupTipeMotorId, i.Amount))
            .ToList()
            .AsReadOnly();

        discount.SetItems(items, command.UpdatedBy);
        await repository.SaveAsync(discount, ct);
    }
}
