namespace Cassandra.Application.Commands.BiayaBiroJasa.SetBiayaBiroJasaStatus;

public record SetBiayaBiroJasaStatusCommand(Guid Id, bool IsActive, string UpdatedBy);
