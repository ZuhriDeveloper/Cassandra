namespace Cassandra.Application.DTOs.Kios;

public record KiosDto(
    Guid    Id,
    string  Code,
    string  Name,
    string  Phone,
    string? Fax,
    string  Address,
    Guid    PicKaryawanId,
    decimal Limit,
    bool    IsActive);
