namespace Cassandra.Application.DTOs.DiscountCash;

public record DiscountCashDto(Guid Id, Guid TipeMotorId, decimal DirectDiscount, decimal ChannelDiscount, bool IsActive);
