using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.DiscountCash;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.DiscountCash.CreateDiscountCash;

public class CreateDiscountCashCommandHandler(
    IDiscountCashRepository repository,
    IDiscountCashQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateDiscountCashCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.TipeMotorIdExistsAsync(command.TipeMotorId, ct))
            throw new DomainException($"Discount cash untuk tipe motor ini sudah ada.");

        var dc = Domain.DiscountCash.DiscountCash.Create(
            command.TipeMotorId, command.DirectDiscount, command.ChannelDiscount, command.CreatedBy, dealerId);
        await repository.SaveAsync(dc, ct);
        return dc.Id.Value;
    }
}
