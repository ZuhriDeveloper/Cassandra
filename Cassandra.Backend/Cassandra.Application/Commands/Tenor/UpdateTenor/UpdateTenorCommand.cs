namespace Cassandra.Application.Commands.Tenor.UpdateTenor;

public record UpdateTenorCommand(Guid Id, string Name, int Months, Guid GrupTenorId, string UpdatedBy);
