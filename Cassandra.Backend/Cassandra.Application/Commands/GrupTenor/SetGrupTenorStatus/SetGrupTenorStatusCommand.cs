namespace Cassandra.Application.Commands.GrupTenor.SetGrupTenorStatus;

public record SetGrupTenorStatusCommand(Guid Id, bool IsActive, string ActionBy);
