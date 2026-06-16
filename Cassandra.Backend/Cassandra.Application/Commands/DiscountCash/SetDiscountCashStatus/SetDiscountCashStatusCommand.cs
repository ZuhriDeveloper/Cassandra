namespace Cassandra.Application.Commands.DiscountCash.SetDiscountCashStatus;

public record SetDiscountCashStatusCommand(Guid Id, bool IsActive, string ActionBy);
