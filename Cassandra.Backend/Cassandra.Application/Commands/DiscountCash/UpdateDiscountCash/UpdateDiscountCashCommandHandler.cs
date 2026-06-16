using Cassandra.Application.Contracts.DiscountCash;
using Cassandra.Domain.Common;
using Cassandra.Domain.DiscountCash;

namespace Cassandra.Application.Commands.DiscountCash.UpdateDiscountCash;

public class UpdateDiscountCashCommandHandler(IDiscountCashRepository repository)
{
    public async Task HandleAsync(UpdateDiscountCashCommand command, CancellationToken ct = default)
    {
        var dc = await repository.GetByIdAsync(DiscountCashId.From(command.Id), ct)
            ?? throw new DomainException($"Discount cash dengan id '{command.Id}' tidak ditemukan.");

        dc.Update(command.DirectDiscount, command.ChannelDiscount, command.UpdatedBy);
        await repository.SaveAsync(dc, ct);
    }
}
