namespace Cassandra.Application.Commands.GlobalLeasing.CreateGlobalLeasing;

public record CreateGlobalLeasingCommand(string Code, string Name, string Phone, string? Fax, string Contact, string Address, string CreatedBy);
