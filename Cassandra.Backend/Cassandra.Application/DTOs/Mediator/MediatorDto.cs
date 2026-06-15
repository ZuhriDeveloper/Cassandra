namespace Cassandra.Application.DTOs.Mediator;

public record MediatorDto(
    Guid    Id,
    string  Name,
    Guid    KaryawanId,
    string  Address,
    decimal Limit,
    bool    IsActive);
