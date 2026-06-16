namespace Cassandra.Application.DTOs.CabangLeasing;

public record CabangLeasingDto(Guid Id, string Code, string Name, string? Phone, string? Fax, string? Contact, Guid GlobalLeasingId, bool IsActive);
