namespace Cassandra.Application.DTOs.Biro;

public record BiroDto(
    Guid Id,
    string Code,
    string Name,
    string? Phone,
    string? Fax,
    string? Pic,
    string? Address,
    decimal PphRate,
    bool IsActive);
