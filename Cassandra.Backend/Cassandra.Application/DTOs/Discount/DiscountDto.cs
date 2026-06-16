namespace Cassandra.Application.DTOs.Discount;

public record DiscountItemDto(Guid GrupTipeMotorId, decimal Amount);

public record DiscountDto(
    Guid Id,
    Guid DaftarHargaLeasingId,
    string Level,
    bool IsActive,
    IReadOnlyList<DiscountItemDto> Items);
