namespace Cassandra.Application.DTOs.TipeMotor;

public record TipeMotorDto(
    Guid Id,
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
    bool IsActive,
    IReadOnlyList<Guid> WarnaIds);
