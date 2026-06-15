namespace Cassandra.Application.Commands.Dealers.SetDealerStatus;

public record SetDealerStatusCommand(Guid DealerId, bool IsActive, string UpdatedBy);
