namespace Cassandra.Application.DTOs.Dealers;

public record DealerDto(
    Guid Id,
    string Name,
    string Code,
    bool IsActive);
