using Cassandra.Domain.Common;

namespace Cassandra.Domain.TipeMotor.Events;

public record TipeMotorCreated(
    TipeMotorId TipeMotorId,
    Guid DealerId,
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
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;
