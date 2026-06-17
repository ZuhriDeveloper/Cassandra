namespace Cassandra.Application.Commands.PengirimanMotor.CreatePengirimanMotor;

public record CreatePengirimanMotorCommand(
    Guid     RegistrasiPenjualanId,
    string   NoMesin,
    Guid     Driver1Id,
    Guid?    Driver2Id,
    DateOnly DeliveryDate,
    string?  Zona,
    string   CreatedBy);
