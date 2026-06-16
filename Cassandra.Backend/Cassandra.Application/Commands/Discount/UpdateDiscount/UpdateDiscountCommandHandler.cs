using Cassandra.Application.Contracts.Discount;
using Cassandra.Domain.Common;
using Cassandra.Domain.Discount;

namespace Cassandra.Application.Commands.Discount.UpdateDiscount;

public class UpdateDiscountCommandHandler(
    IDiscountRepository repository,
    IDiscountQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateDiscountCommand command, CancellationToken ct = default)
    {
        var discount = await repository.GetByIdAsync(DiscountId.From(command.Id), ct)
            ?? throw new DomainException($"Discount dengan id '{command.Id}' tidak ditemukan.");

        if (await queryRepository.ExistsExcludingAsync(command.DaftarHargaLeasingId, command.Level.Trim(), command.Id, ct))
            throw new DomainException($"Discount level '{command.Level}' sudah ada untuk daftar harga leasing ini.");

        discount.Update(command.DaftarHargaLeasingId, command.Level, command.UpdatedBy);
        await repository.SaveAsync(discount, ct);
    }
}
