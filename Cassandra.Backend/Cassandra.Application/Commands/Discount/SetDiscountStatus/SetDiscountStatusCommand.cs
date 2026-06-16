namespace Cassandra.Application.Commands.Discount.SetDiscountStatus;

public record SetDiscountStatusCommand(Guid Id, bool IsActive, string ActionBy);
