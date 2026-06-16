namespace Cassandra.Application.Commands.CabangLeasing.SetCabangLeasingStatus;

public record SetCabangLeasingStatusCommand(Guid Id, bool IsActive, string ActionBy);
