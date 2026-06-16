namespace Cassandra.Application.Commands.GlobalLeasing.UpdateGlobalLeasing;

public record UpdateGlobalLeasingCommand(Guid Id, string Name, string Phone, string? Fax, string Contact, string Address, string UpdatedBy);
