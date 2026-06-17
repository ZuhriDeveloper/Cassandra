namespace Cassandra.Application.DTOs.PengirimanMotor;

public record PengirimanMotorDto(
    Guid     Id,
    Guid     RegistrasiPenjualanId,
    string   NoMesin,
    Guid     Driver1Id,
    Guid?    Driver2Id,
    DateOnly DeliveryDate,
    string?  Zona,
    string   CreatedBy,
    DateTime CreatedAt);
