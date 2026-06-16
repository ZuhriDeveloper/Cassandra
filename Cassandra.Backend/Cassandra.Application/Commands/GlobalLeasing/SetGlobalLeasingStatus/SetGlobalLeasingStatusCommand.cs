namespace Cassandra.Application.Commands.GlobalLeasing.SetGlobalLeasingStatus;

public record SetGlobalLeasingStatusCommand(Guid Id, bool IsActive, string ActionBy);
