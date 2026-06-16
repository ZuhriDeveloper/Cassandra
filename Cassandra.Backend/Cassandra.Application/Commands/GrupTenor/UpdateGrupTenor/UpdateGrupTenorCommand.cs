namespace Cassandra.Application.Commands.GrupTenor.UpdateGrupTenor;

public record UpdateGrupTenorCommand(Guid Id, string Name, string UpdatedBy);
