namespace Cassandra.Application.Commands.DiscountCash.CreateDiscountCash;

public record CreateDiscountCashCommand(Guid TipeMotorId, decimal DirectDiscount, decimal ChannelDiscount, string CreatedBy);
