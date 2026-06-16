using Cassandra.Application.Contracts.Discount;
using Cassandra.Domain.Common;
using Cassandra.Domain.Discount;

namespace Cassandra.Application.Commands.Discount.SetDiscountStatus;

public class SetDiscountStatusCommandHandler(IDiscountRepository repository)
{
    public async Task HandleAsync(SetDiscountStatusCommand command, CancellationToken ct = default)
    {
        var discount = await repository.GetByIdAsync(DiscountId.From(command.Id), ct)
            ?? throw new DomainException($"Discount dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            discount.Activate(command.ActionBy);
        else
            discount.Deactivate(command.ActionBy);

        await repository.SaveAsync(discount, ct);
    }
}
