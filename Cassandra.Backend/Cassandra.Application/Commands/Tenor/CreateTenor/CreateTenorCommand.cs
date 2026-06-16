namespace Cassandra.Application.Commands.Tenor.CreateTenor;

public record CreateTenorCommand(string Code, string Name, int Months, Guid GrupTenorId, string CreatedBy);
