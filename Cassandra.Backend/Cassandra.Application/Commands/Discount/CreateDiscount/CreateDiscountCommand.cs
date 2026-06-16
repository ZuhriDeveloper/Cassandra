namespace Cassandra.Application.Commands.Discount.CreateDiscount;

public record CreateDiscountCommand(Guid DaftarHargaLeasingId, string Level, string CreatedBy);
