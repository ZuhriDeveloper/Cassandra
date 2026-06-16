namespace Cassandra.Application.Commands.Discount.SetDiscountItems;

public record SetDiscountItem(Guid GrupTipeMotorId, decimal Amount);

public record SetDiscountItemsCommand(Guid Id, List<SetDiscountItem> Items, string UpdatedBy);
