namespace Cassandra.Application.Commands.DiscountCash.UpdateDiscountCash;

public record UpdateDiscountCashCommand(Guid Id, decimal DirectDiscount, decimal ChannelDiscount, string UpdatedBy);
