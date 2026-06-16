namespace Cassandra.Application.DTOs.GlobalLeasing;

public record GlobalLeasingDto(Guid Id, string Code, string Name, string Phone, string? Fax, string Contact, string Address, bool IsActive);
