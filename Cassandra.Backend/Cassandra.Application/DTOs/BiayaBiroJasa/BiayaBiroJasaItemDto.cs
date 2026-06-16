namespace Cassandra.Application.DTOs.BiayaBiroJasa;

public record BiayaBiroJasaItemDto(
    Guid TipeMotorId,
    decimal BiayaStnk,
    decimal Notice);
