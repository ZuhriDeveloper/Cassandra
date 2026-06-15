namespace Cassandra.Application.Commands.TipeMotor.UpdateTipeMotor;

public record UpdateTipeMotorCommand(
    Guid Id,
    Guid GrupTipeMotorId,
    string ShortName,
    string ProductCode,
    string WmsCode,
    string AhmCode,
    string EngineNumberFormat,
    string ChassisNumberFormat,
    decimal NettPrice,
    decimal OrJakarta,
    decimal OrTangerang,
    decimal BbnJakarta,
    decimal BbnTangerang,
    string UpdatedBy);
