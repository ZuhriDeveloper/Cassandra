using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Discount;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Discount.CreateDiscount;

public class CreateDiscountCommandHandler(
    IDiscountRepository repository,
    IDiscountQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateDiscountCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.ExistsAsync(command.DaftarHargaLeasingId, command.Level.Trim(), ct))
            throw new DomainException($"Discount level '{command.Level}' sudah ada untuk daftar harga leasing ini.");

        var discount = Domain.Discount.Discount.Create(command.DaftarHargaLeasingId, command.Level, command.CreatedBy, dealerId);
        await repository.SaveAsync(discount, ct);
        return discount.Id.Value;
    }
}
