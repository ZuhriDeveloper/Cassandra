namespace Cassandra.Application.Commands.CabangLeasing.UpdateCabangLeasing;

public record UpdateCabangLeasingCommand(Guid Id, string Name, string? Phone, string? Fax, string? Contact, Guid GlobalLeasingId, string UpdatedBy);
