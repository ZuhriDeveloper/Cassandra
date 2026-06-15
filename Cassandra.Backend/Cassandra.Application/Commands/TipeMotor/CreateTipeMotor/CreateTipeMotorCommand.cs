namespace Cassandra.Application.Commands.TipeMotor.CreateTipeMotor;

public record CreateTipeMotorCommand(
    string Code,
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
    string CreatedBy);
