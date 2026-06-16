namespace Cassandra.Application.Commands.CabangLeasing.CreateCabangLeasing;

public record CreateCabangLeasingCommand(string Code, string Name, string? Phone, string? Fax, string? Contact, Guid GlobalLeasingId, string CreatedBy);
