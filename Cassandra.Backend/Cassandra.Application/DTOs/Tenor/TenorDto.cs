namespace Cassandra.Application.DTOs.Tenor;

public record TenorDto(Guid Id, string Code, string Name, int Months, Guid GrupTenorId, bool IsActive);
