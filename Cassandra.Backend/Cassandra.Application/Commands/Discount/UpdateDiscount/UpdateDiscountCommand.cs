namespace Cassandra.Application.Commands.Discount.UpdateDiscount;

public record UpdateDiscountCommand(Guid Id, Guid DaftarHargaLeasingId, string Level, string UpdatedBy);
